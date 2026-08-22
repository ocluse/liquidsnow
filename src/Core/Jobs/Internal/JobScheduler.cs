using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Ocluse.LiquidSnow.Jobs.Persistence;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed class JobScheduler : IJobScheduler, IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJobStore _store;
    private readonly IJobSerializer _serializer;
    private readonly IJobKeySerializer _keySerializer;
    private readonly JobsOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<JobScheduler> _logger;
    private readonly string _workerId = $"{Environment.MachineName}:{Environment.ProcessId}:{Guid.NewGuid():N}";
    private readonly CancellationTokenSource _stopping = new();
    private readonly SemaphoreSlim _wakeSignal = new(0, 1);
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly SemaphoreSlim _executionSlots;
    private readonly ConcurrentDictionary<(string Scope, string Id), CancellationTokenSource> _activeJobs = [];
    private readonly ConcurrentDictionary<long, Task> _executions = [];
    private readonly object _workerSync = new();
    private Task? _workerTask;
    private bool _initialized;
    private long _executionId;

    public JobScheduler(
        IServiceProvider serviceProvider,
        IJobStore store,
        IJobSerializer serializer,
        IJobKeySerializer keySerializer,
        JobsOptions options,
        TimeProvider timeProvider,
        ILogger<JobScheduler>? logger = null)
    {
        ValidateOptions(options);
        _serviceProvider = serviceProvider;
        _store = store;
        _serializer = serializer;
        _keySerializer = keySerializer;
        _options = options;
        _timeProvider = timeProvider;
        _logger = logger ?? NullLogger<JobScheduler>.Instance;
        _executionSlots = new SemaphoreSlim(options.MaximumConcurrency, options.MaximumConcurrency);
    }

    public IDisposable Schedule<T>(T job) where T : IJob =>
        ScheduleAsync(job).AsTask().GetAwaiter().GetResult();

    public ValueTask<IJobHandle> ScheduleAsync<T>(T job, CancellationToken cancellationToken = default) where T : IJob =>
        StoreAsync(job, null, cancellationToken);

    public IDisposable Queue<T>(T job) where T : IQueueJob =>
        QueueAsync(job).AsTask().GetAwaiter().GetResult();

    public ValueTask<IJobHandle> QueueAsync<T>(T job, CancellationToken cancellationToken = default) where T : IQueueJob
    {
        ArgumentNullException.ThrowIfNull(job);
        return StoreAsync(job, _keySerializer.Serialize(job.QueueId), cancellationToken);
    }

    public bool Cancel(object id) => CancelAsync(id).AsTask().GetAwaiter().GetResult();

    public ValueTask<bool> CancelAsync(object id, CancellationToken cancellationToken = default) =>
        CancelStoredAsync(_keySerializer.Serialize(id), null, cancellationToken);

    public bool Cancel(object queueId, object id) =>
        CancelAsync(queueId, id).AsTask().GetAwaiter().GetResult();

    public ValueTask<bool> CancelAsync(object queueId, object id, CancellationToken cancellationToken = default) =>
        CancelStoredAsync(_keySerializer.Serialize(id), _keySerializer.Serialize(queueId), cancellationToken);

    internal async Task StartAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        EnsureWorkerStarted();
        SignalWorker();
    }

    internal async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_stopping.IsCancellationRequested)
        {
            await _stopping.CancelAsync();
        }

        Task? workerTask;
        lock (_workerSync)
        {
            workerTask = _workerTask;
        }

        if (workerTask is not null)
        {
            await workerTask.WaitAsync(cancellationToken);
        }

        Task[] executions = _executions.Values.ToArray();
        if (executions.Length > 0)
        {
            await Task.WhenAll(executions).WaitAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await StopAsync(CancellationToken.None);
        }
        finally
        {
            _stopping.Dispose();
            _wakeSignal.Dispose();
            _initializationLock.Dispose();
            _executionSlots.Dispose();
        }
    }

    private async ValueTask<IJobHandle> StoreAsync(IJob job, string? queueId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(job);
        await EnsureInitializedAsync(cancellationToken);

        SerializedJob serialized = _serializer.Serialize(job);
        JobScheduleKind scheduleKind = GetScheduleKind(job);
        TimeSpan? interval = job is IRoutineJob routineJob ? routineJob.Interval : null;
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(job), "Recurring job intervals must be greater than zero.");
        }

        string id = _keySerializer.Serialize(job.Id);
        JobStoreRecord record = new()
        {
            Id = id,
            QueueId = queueId,
            TypeName = serialized.TypeName,
            Payload = serialized.Payload,
            ScheduleKind = scheduleKind,
            DueAt = job.Start,
            Interval = interval,
            Tick = 0
        };

        await _store.StoreAsync(record, cancellationToken);
        EnsureWorkerStarted();
        SignalWorker();
        return new JobHandle(this, id, queueId);
    }

    private async ValueTask<bool> CancelStoredAsync(string id, string? queueId, CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken);
        bool cancelled = await _store.CancelAsync(id, queueId, cancellationToken);

        if (cancelled && _activeJobs.TryGetValue(GetActiveKey(id, queueId), out CancellationTokenSource? activeJob))
        {
            await activeJob.CancelAsync();
        }

        SignalWorker();
        return cancelled;
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (!_initialized)
            {
                await _store.InitializeAsync(cancellationToken);
                _initialized = true;
            }
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private void EnsureWorkerStarted()
    {
        lock (_workerSync)
        {
            ObjectDisposedException.ThrowIf(_stopping.IsCancellationRequested, this);
            _workerTask ??= RunWorkerAsync(_stopping.Token);
        }
    }

    private async Task RunWorkerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    int availableSlots = Math.Max(0, _options.MaximumConcurrency - _executions.Count);
                    if (availableSlots > 0)
                    {
                        JobClaimRequest request = new(
                            _timeProvider.GetUtcNow(),
                            _workerId,
                            _options.LeaseDuration,
                            Math.Min(_options.ClaimBatchSize, availableSlots));

                        IReadOnlyList<JobStoreRecord> jobs = await _store.ClaimDueAsync(request, cancellationToken);
                        foreach (JobStoreRecord job in jobs)
                        {
                            await _executionSlots.WaitAsync(cancellationToken);
                            TrackExecution(job, cancellationToken);
                        }

                        if (jobs.Count > 0)
                        {
                            continue;
                        }
                    }

                    await WaitForWorkAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "The durable jobs worker encountered a store error.");
                    await Task.Delay(_options.PollInterval, _timeProvider, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private void TrackExecution(JobStoreRecord job, CancellationToken stoppingToken)
    {
        long executionId = Interlocked.Increment(ref _executionId);
        Task execution = ExecuteAndReleaseAsync(job, stoppingToken);
        _executions[executionId] = execution;
        _ = execution.ContinueWith(
            _ =>
            {
                _executions.TryRemove(executionId, out Task? _);
                SignalWorker();
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    private async Task ExecuteAndReleaseAsync(JobStoreRecord record, CancellationToken stoppingToken)
    {
        try
        {
            await ExecuteAsync(record, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Execution state for durable job {JobId} could not be persisted.", record.Id);
        }
        finally
        {
            _executionSlots.Release();
        }
    }

    private async Task ExecuteAsync(JobStoreRecord record, CancellationToken stoppingToken)
    {
        JobLease lease = new(record.Id, record.QueueId, _workerId, record.Version);
        (string Scope, string Id) activeKey = GetActiveKey(record.Id, record.QueueId);
        using CancellationTokenSource executionCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        if (!_activeJobs.TryAdd(activeKey, executionCancellation))
        {
            using CancellationTokenSource releaseTimeout = new(TimeSpan.FromSeconds(5));
            await _store.CompleteAsync(
                new JobCompletion(lease, record.DueAt, record.Tick, "Another execution with the same identity is still active."),
                releaseTimeout.Token);
            return;
        }

        using CancellationTokenSource heartbeatCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        Task heartbeat = RenewLeaseAsync(lease, executionCancellation, heartbeatCancellation.Token);
        string? error = null;
        bool interrupted = false;

        try
        {
            IJob job = _serializer.Deserialize(record.TypeName, record.Payload);
            using IServiceScope scope = _serviceProvider.CreateScope();
            IJobDispatcher dispatcher = scope.ServiceProvider.GetRequiredService<IJobDispatcher>();
            await dispatcher.DispatchAsync(job, job.GetType(), record.Tick, true, executionCancellation.Token);
        }
        catch (OperationCanceledException) when (executionCancellation.IsCancellationRequested)
        {
            interrupted = true;
        }
        catch (Exception exception)
        {
            error = exception.ToString();
        }
        finally
        {
            await heartbeatCancellation.CancelAsync();
            try
            {
                await heartbeat;
            }
            catch (OperationCanceledException) when (heartbeatCancellation.IsCancellationRequested)
            {
            }
            finally
            {
                _activeJobs.TryRemove(activeKey, out CancellationTokenSource? _);
            }
        }

        if (interrupted || stoppingToken.IsCancellationRequested || executionCancellation.IsCancellationRequested)
        {
            using CancellationTokenSource releaseTimeout = new(TimeSpan.FromSeconds(5));
            try
            {
                await _store.CompleteAsync(
                    new JobCompletion(lease, record.DueAt, record.Tick, "Execution was interrupted before completion."),
                    releaseTimeout.Token);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "The lease for interrupted job {JobId} could not be released.", record.Id);
            }

            return;
        }

        await _store.CompleteAsync(
            new JobCompletion(lease, GetNextDueAt(record), record.Tick + 1, error),
            stoppingToken);
    }

    private async Task RenewLeaseAsync(
        JobLease lease,
        CancellationTokenSource executionCancellation,
        CancellationToken cancellationToken)
    {
        TimeSpan heartbeatInterval = TimeSpan.FromTicks(Math.Max(1, _options.LeaseDuration.Ticks / 3));

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(heartbeatInterval, _timeProvider, cancellationToken);
            DateTimeOffset expiresAt = _timeProvider.GetUtcNow() + _options.LeaseDuration;
            bool renewed = await _store.RenewLeaseAsync(lease, expiresAt, cancellationToken);
            if (!renewed)
            {
                await executionCancellation.CancelAsync();
                return;
            }
        }
    }

    private DateTimeOffset? GetNextDueAt(JobStoreRecord job)
    {
        if (job.ScheduleKind == JobScheduleKind.OneTime)
        {
            return null;
        }

        TimeSpan interval = job.Interval
            ?? throw new InvalidOperationException("A recurring job must have an interval.");

        if (job.ScheduleKind == JobScheduleKind.TaskSeries)
        {
            return _timeProvider.GetUtcNow() + interval;
        }

        DateTimeOffset next = job.DueAt + interval;
        DateTimeOffset now = _timeProvider.GetUtcNow();
        if (next <= now)
        {
            long intervalsToSkip = ((now - next).Ticks / interval.Ticks) + 1;
            next += TimeSpan.FromTicks(checked(interval.Ticks * intervalsToSkip));
        }

        return next;
    }

    private async Task WaitForWorkAsync(CancellationToken cancellationToken)
    {
        TimeSpan delay = _options.PollInterval;
        DateTimeOffset? nextDueTime = await _store.GetNextDueTimeAsync(cancellationToken);
        if (nextDueTime is not null)
        {
            TimeSpan untilDue = nextDueTime.Value - _timeProvider.GetUtcNow();
            delay = untilDue <= TimeSpan.Zero
                ? _options.PollInterval
                : (untilDue < delay ? untilDue : delay);
        }

        using CancellationTokenSource waitCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task delayTask = Task.Delay(delay, _timeProvider, waitCancellation.Token);
        Task signalTask = _wakeSignal.WaitAsync(waitCancellation.Token);
        await Task.WhenAny(delayTask, signalTask);
        await waitCancellation.CancelAsync();
    }

    private void SignalWorker()
    {
        if (_wakeSignal.CurrentCount != 0)
        {
            return;
        }

        try
        {
            _wakeSignal.Release();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (SemaphoreFullException)
        {
        }
    }

    private static JobScheduleKind GetScheduleKind(IJob job) => job switch
    {
        ITaskSeriesJob => JobScheduleKind.TaskSeries,
        IRoutineJob => JobScheduleKind.FixedRate,
        _ => JobScheduleKind.OneTime
    };

    private static (string Scope, string Id) GetActiveKey(string id, string? queueId) =>
        (queueId ?? string.Empty, id);

    private static void ValidateOptions(JobsOptions options)
    {
        if (options.PollInterval <= TimeSpan.Zero || options.LeaseDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Job polling and lease durations must be greater than zero.");
        }

        if (options.ClaimBatchSize <= 0 || options.MaximumConcurrency <= 0) 
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Job batch size and concurrency must be greater than zero.");
        }
    }

    private sealed class JobHandle(JobScheduler scheduler, string id, string? queueId) : IJobHandle
    {
        private int _disposed;

        public string Id { get; } = id;

        public string? QueueId { get; } = queueId;

        public ValueTask<bool> CancelAsync(CancellationToken cancellationToken = default) =>
            scheduler.CancelStoredAsync(Id, QueueId, cancellationToken);

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                CancelAsync().AsTask().GetAwaiter().GetResult();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                await CancelAsync();
            }
        }
    }
}

internal sealed class JobSchedulerHostedService(JobScheduler scheduler) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => scheduler.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => scheduler.StopAsync(cancellationToken);
}
