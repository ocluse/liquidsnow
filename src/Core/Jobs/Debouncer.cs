namespace Ocluse.LiquidSnow.Jobs;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a debouncing mechanism that delays execution of an action until a specified interval elapses
/// without further calls. Supports async actions, cancellation, throttling, queueing, and generic payloads.
/// </summary>
public sealed class Debouncer<T> : IDisposable
{
    private readonly TimeSpan _interval;
    private readonly TimeSpan? _maxWait;
    private readonly Func<T, CancellationToken, Task>? _asyncAction;
    private readonly Action<T, CancellationToken>? _syncAction;
    private readonly bool _isThrottled;
    private readonly bool _isQueued;
    private readonly bool _originalFireImmediately;

    private bool _fireImmediately;
    private IDisposable? _debounceSubscription;
    private IDisposable? _maxWaitSubscription;
    private CancellationTokenSource? _cts;
    private T? _latestValue = default!;
    private DateTime _lastExecution = DateTime.MinValue;
    private readonly Queue<(T, CancellationTokenSource)> _queue = new();
    private bool _isProcessingQueue;
    private bool _running;
    private bool _disposed;

    /// <summary>
    /// Occurs when a queued or direct debounce execution fails.
    /// </summary>
    public event EventHandler<Exception>? ExecutionFailed;

    /// <summary>
    /// Indicates whether the debouncer is actively waiting or processing.
    /// </summary>
    public bool Running => _running;

    /// <summary>
    /// Indicates whether the debouncer throttles actions to prevent rapid execution.
    /// </summary>
    public bool IsThrottled => _isThrottled;

    /// <summary>
    /// Indicates whether the debouncer queues its actions so that they are called sequentially.
    /// </summary>
    public bool IsQueued => _isQueued;

    /// <summary>
    /// Indicates whether the debouncer is currently processing a queue of actions.
    /// </summary>
    public bool IsProcessingQueue => _isProcessingQueue;

    /// <summary>
    /// Indicates whether the debouncer fires the first action immediately.
    /// </summary>
    public bool FireImmediately => _fireImmediately;

    /// <summary>
    /// Retrieves the time the most recent action was executed, in UTC.
    /// </summary>
    public DateTime LastExecution => _lastExecution;

    /// <summary>
    /// Gets the last value that was passed to the debouncer.
    /// </summary>
    /// <remarks>
    /// This is only available when <see cref="IsQueued"/> is false.
    /// </remarks>
    public T? LatestValue => _latestValue;

    /// <summary>
    /// Initializes a new instance of the Debouncer with an async action.
    /// </summary>
    /// <param name="intervalMs">The debounce delay interval in milliseconds</param>
    /// <param name="asyncAction">The async action to execute after the debounce interval</param>
    /// <param name="fireImmediately">If true, runs the action immediately on the first call</param>
    /// <param name="maxWait">Optional maximum delay before the action must run</param>
    /// <param name="isThrottled">If true, the debouncer throttles actions to prevent rapid execution.</param>
    /// <param name="isQueued">If true, the debouncer adds its actions to a queue for sequential processing.</param>
    public Debouncer(
        double intervalMs,
        Func<T, CancellationToken, Task> asyncAction,
        bool fireImmediately = false,
        TimeSpan? maxWait = null,
        bool isThrottled = false,
        bool isQueued = false)
    {
        _interval = TimeSpan.FromMilliseconds(intervalMs);
        _asyncAction = asyncAction;
        _fireImmediately = fireImmediately;
        _originalFireImmediately = fireImmediately;
        _maxWait = maxWait;
        _isThrottled = isThrottled;
        _isQueued = isQueued;
    }

    /// <summary>
    /// Initializes a new instance of the Debouncer with a sync action.
    /// </summary>
    /// <param name="intervalMs">The debounce delay interval in milliseconds</param>
    /// <param name="syncAction">The sync action to execute after the debounce interval</param>
    /// <param name="fireImmediately">If true, runs the action immediately on the first call</param>
    /// <param name="maxWait">Optional maximum delay before the action must run</param>
    /// <param name="isThrottled">If true, the debouncer throttles actions to prevent rapid execution.</param>
    /// <param name="isQueued">If true, the debouncer adds its actions to a queue for sequential processing.</param>
    public Debouncer(
        double intervalMs,
        Action<T, CancellationToken> syncAction,
        bool fireImmediately = false,
        TimeSpan? maxWait = null,
        bool isThrottled = false,
        bool isQueued = false)
    {
        _interval = TimeSpan.FromMilliseconds(intervalMs);
        _syncAction = syncAction;
        _fireImmediately = fireImmediately;
        _originalFireImmediately = fireImmediately;
        _maxWait = maxWait;
        _isThrottled = isThrottled;
        _isQueued = isQueued;
    }

    /// <summary>
    /// Starts or resets the debounce timer with the given value.
    /// </summary>
    /// <param name="value">The value to pass to the action when it is executed.</param>
    /// <remarks>
    /// If <see cref="IsQueued"/> is enabled, the input is added tot he queue for sequential processing.<br/>
    /// If <see cref="IsThrottled"/> is enabled and the previous action was executed recently, the new input is ignored.<br/>
    /// If <see cref="FireImmediately"/> is enabled and its the first call, the action is executed immediately<br/>
    /// </remarks>
    public void Begin(T value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_isThrottled && (DateTime.UtcNow - _lastExecution) < _interval)
        {
            if (_isQueued)
            {
                AddToQueue(value);
            }
            return; // throttled
        }

        if (_isQueued)
        {
            AddToQueue(value);
            return;
        }

        Cancel();

        _running = true;
        _latestValue = value;
        _cts = new CancellationTokenSource();

        if (_fireImmediately)
        {
            _fireImmediately = false;
            _ = ExecuteAsync(value, _cts.Token);
            return;
        }

        _debounceSubscription = Observable
            .Timer(_interval)
            .Subscribe(async (_) => await ExecuteAsync(value, _cts.Token));

        if (_maxWait.HasValue)
        {
            _maxWaitSubscription = Observable
                .Timer(_maxWait.Value)
                .Subscribe(async (_) => await ExecuteAsync(value, _cts.Token));
        }
    }

    private void AddToQueue(T value)
    {
        var cts = new CancellationTokenSource();
        _queue.Enqueue((value, cts));
        if (!_isProcessingQueue)
            _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {
        _isProcessingQueue = true;

        while (_queue.Count > 0)
        {
            var (value, cts) = _queue.Dequeue();
            _lastExecution = DateTime.UtcNow;
            _running = true;

            try
            {
                if (_asyncAction != null)
                    await _asyncAction(value, cts.Token);
                else
                    _syncAction?.Invoke(value, cts.Token);
            }
            catch (Exception ex)
            {
                ExecutionFailed?.Invoke(this, ex);
            }

            await Task.Delay(_interval);
        }

        _isProcessingQueue = false;
        _running = false;
    }

    private async Task ExecuteAsync(T value, CancellationToken token)
    {
        if (!_running || token.IsCancellationRequested) return;

        _running = false;
        _lastExecution = DateTime.UtcNow;

        try
        {
            if (_asyncAction != null)
                await _asyncAction(value, token);
            else
                _syncAction?.Invoke(value, token);
        }
        catch (Exception ex)
        {
            ExecutionFailed?.Invoke(this, ex);
        }

        Cancel();
    }

    /// <summary>
    /// Cancels any pending debounce operations.
    /// </summary>
    /// <remarks>
    /// This only works to cancel if the debounce is not queueing its actions. i.e. <see cref="IsQueued"/> is false.
    /// </remarks>
    public void Cancel()
    {
        if (_disposed) return;

        _running = false;

        _debounceSubscription?.Dispose();
        _debounceSubscription = null;

        _maxWaitSubscription?.Dispose();
        _maxWaitSubscription = null;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    /// <summary>
    /// Resets the debouncer to its initial state, clearing queues and flags.
    /// </summary>
    public void Reset()
    {
        Cancel();

        _queue.Clear();
        _fireImmediately = _originalFireImmediately;
        _lastExecution = DateTime.MinValue;
        _isProcessingQueue = false;
    }

    /// <summary>
    /// Disposes all internal resources and prevents further use.
    /// </summary>
    public void Dispose()
    {
        Reset();
        _disposed = true;
    }
}

/// <summary>
/// A non-generic version of Debouncer for scenarios without payloads.
/// </summary>
public sealed class Debouncer : IDisposable
{
    private readonly Debouncer<object?> _inner;

    ///<inheritdoc cref="Debouncer{T}.ExecutionFailed"/>
    public event EventHandler<Exception>? ExecutionFailed
    {
        add => _inner.ExecutionFailed += value;
        remove => _inner.ExecutionFailed -= value;
    }

    ///<inheritdoc cref="Debouncer{T}.Running"/>
    public bool Running => _inner.Running;

    ///<inheritdoc cref="Debouncer{T}.IsThrottled"/>
    public bool IsThrottled => _inner.IsThrottled;

    ///<inheritdoc cref="Debouncer{T}.IsQueued"/>
    public bool IsQueued => _inner.IsQueued;

    ///<inheritdoc cref="Debouncer{T}.IsProcessingQueue"/>
    public bool IsProcessingQueue => _inner.IsProcessingQueue;

    ///<inheritdoc cref="Debouncer{T}.FireImmediately"/>
    public bool FireImmediately => _inner.FireImmediately;

    ///<inheritdoc cref="Debouncer{T}.LastExecution"/>
    public DateTime LastExecution => _inner.LastExecution;

    ///<inheritdoc cref="Debouncer{T}.Debouncer(double, Func{T, CancellationToken, Task}, bool, TimeSpan?, bool, bool)"/>
    public Debouncer(
        double intervalMs,
        Func<CancellationToken, Task> asyncAction,
        bool fireImmediately = false,
        TimeSpan? maxWait = null,
        bool isThrottled = false,
        bool isQueued = false)
    {
        _inner = new Debouncer<object?>(
            intervalMs,
            (_, ct) => asyncAction(ct),
            fireImmediately,
            maxWait,
            isThrottled,
            isQueued
        );
    }

    /// <inheritdoc cref="Debouncer{T}.Debouncer(double, Action{T, CancellationToken}, bool, TimeSpan?, bool, bool)"/>
    public Debouncer(
        double intervalMs,
        Action<CancellationToken> syncAction,
        bool fireImmediately = false,
        TimeSpan? maxWait = null,
        bool isThrottled = false,
        bool isQueued = false)
    {
        _inner = new Debouncer<object?>(
            intervalMs,
            (_, ct) => syncAction(ct),
            fireImmediately,
            maxWait,
            isThrottled,
            isQueued
        );
    }

    ///<inheritdoc cref="Debouncer{T}.Begin(T)"/>
    public void Begin() => _inner.Begin(null);

    ///<inheritdoc cref="Debouncer{T}.Cancel"/>
    public void Cancel() => _inner.Cancel();

    ///<inheritdoc cref="Debouncer{T}.Reset"/>
    public void Reset() => _inner.Reset();

    ///<inheritdoc cref="Debouncer{T}.Dispose"/>
    public void Dispose() => _inner.Dispose();
}
