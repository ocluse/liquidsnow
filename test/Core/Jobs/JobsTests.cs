using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Ocluse.LiquidSnow.Jobs;
using Ocluse.LiquidSnow.Jobs.Persistence;

namespace Ocluse.LiquidSnow.Core.Tests.Jobs;

public sealed class JobsTests
{
    [Fact]
    public async Task Scheduler_can_be_resolved_without_a_generic_host()
    {
        ServiceCollection services = new();
        services.AddSingleton<JobProbe>();
        services.AddJobs(typeof(JobsTests).Assembly);

        await using ServiceProvider provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        Assert.NotNull(provider.GetRequiredService<IJobScheduler>());
    }

    [Fact]
    public async Task Scheduled_job_is_dispatched()
    {
        JobProbe probe = new();
        using IHost host = await CreateHostAsync(probe);
        IJobScheduler scheduler = host.Services.GetRequiredService<IJobScheduler>();

        await scheduler.ScheduleAsync(new TestJob("one", DateTimeOffset.UtcNow, 42));

        Assert.Equal(42, await probe.ReadAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Durable_cancel_prevents_execution()
    {
        JobProbe probe = new();
        using IHost host = await CreateHostAsync(probe);
        IJobScheduler scheduler = host.Services.GetRequiredService<IJobScheduler>();

        await scheduler.ScheduleAsync(new TestJob("cancelled", DateTimeOffset.UtcNow.AddMilliseconds(300), 1));
        Assert.True(await scheduler.CancelAsync("cancelled"));

        await Task.Delay(500);
        Assert.Empty(probe.Values);
    }

    [Fact]
    public async Task Stored_job_is_recovered_by_a_new_host()
    {
        InMemoryJobStore store = new();
        JobProbe probe = new();
        DateTimeOffset dueAt = DateTimeOffset.UtcNow.AddMilliseconds(250);

        using (IHost firstHost = await CreateHostAsync(probe, store))
        {
            IJobScheduler scheduler = firstHost.Services.GetRequiredService<IJobScheduler>();
            await scheduler.ScheduleAsync(new TestJob("recover", dueAt, 7));
            await firstHost.StopAsync();
        }

        using IHost secondHost = await CreateHostAsync(probe, store);
        Assert.Equal(7, await probe.ReadAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Queued_jobs_keep_their_durable_order()
    {
        JobProbe probe = new();
        using IHost host = await CreateHostAsync(probe);
        IJobScheduler scheduler = host.Services.GetRequiredService<IJobScheduler>();
        DateTimeOffset dueAt = DateTimeOffset.UtcNow.AddMilliseconds(100);

        await scheduler.QueueAsync(new TestQueueJob("first", "emails", dueAt, 1));
        await scheduler.QueueAsync(new TestQueueJob("second", "emails", dueAt, 2));

        Assert.Equal(1, await probe.ReadAsync(CancellationToken.None));
        Assert.Equal(2, await probe.ReadAsync(CancellationToken.None));
    }

    [Fact]
    public async Task Recurring_job_advances_its_tick_and_can_be_cancelled()
    {
        JobProbe probe = new();
        using IHost host = await CreateHostAsync(probe);
        IJobScheduler scheduler = host.Services.GetRequiredService<IJobScheduler>();

        IJobHandle handle = await scheduler.ScheduleAsync(
            new TestRoutineJob("routine", DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(50)));

        Assert.Equal(0, await probe.ReadAsync(CancellationToken.None));
        Assert.Equal(1, await probe.ReadAsync(CancellationToken.None));
        Assert.True(await handle.CancelAsync());
    }

    private static async Task<IHost> CreateHostAsync(JobProbe probe, IJobStore? store = null)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder()
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton(probe);
                services.AddJobs(typeof(JobsTests).Assembly)
                    .Configure(options => options.PollInterval = TimeSpan.FromMilliseconds(25));

                if (store is not null)
                {
                    services.RemoveAll<IJobStore>();
                    services.AddSingleton(store);
                }
            });

        IHost host = builder.Build();
        await host.StartAsync();
        return host;
    }
}

public sealed class JobStoreTests
{
    [Fact]
    public async Task Queue_claims_only_one_head_at_a_time()
    {
        InMemoryJobStore store = new();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await store.StoreAsync(CreateRecord("first", "queue", now));
        await store.StoreAsync(CreateRecord("second", "queue", now));

        IReadOnlyList<JobStoreRecord> firstClaim = await store.ClaimDueAsync(
            new JobClaimRequest(now, "worker", TimeSpan.FromMinutes(1), 10));

        JobStoreRecord first = Assert.Single(firstClaim);
        Assert.Equal("first", first.Id);
        Assert.True(await store.CompleteAsync(
            new JobCompletion(new JobLease(first.Id, first.QueueId, "worker", first.Version), null, 1, null)));

        IReadOnlyList<JobStoreRecord> secondClaim = await store.ClaimDueAsync(
            new JobClaimRequest(now, "worker", TimeSpan.FromMinutes(1), 10));
        Assert.Equal("second", Assert.Single(secondClaim).Id);
    }

    [Fact]
    public async Task Expired_lease_is_recovered_and_old_worker_cannot_complete()
    {
        InMemoryJobStore store = new();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        await store.StoreAsync(CreateRecord("job", null, now));

        JobStoreRecord first = Assert.Single(await store.ClaimDueAsync(
            new JobClaimRequest(now, "worker-1", TimeSpan.FromSeconds(1), 1)));
        JobStoreRecord recovered = Assert.Single(await store.ClaimDueAsync(
            new JobClaimRequest(now.AddSeconds(2), "worker-2", TimeSpan.FromSeconds(1), 1)));

        Assert.False(await store.CompleteAsync(
            new JobCompletion(new JobLease(first.Id, null, "worker-1", first.Version), null, 1, null)));
        Assert.True(await store.CompleteAsync(
            new JobCompletion(new JobLease(recovered.Id, null, "worker-2", recovered.Version), null, 1, null)));
    }

    private static JobStoreRecord CreateRecord(string id, string? queueId, DateTimeOffset dueAt) => new()
    {
        Id = id,
        QueueId = queueId,
        TypeName = "test",
        Payload = [],
        ScheduleKind = JobScheduleKind.OneTime,
        DueAt = dueAt,
        Tick = 0
    };
}

public sealed class JobKeySerializerTests
{
    [Fact]
    public void Unsupported_compound_identifier_requires_a_custom_serializer()
    {
        DefaultJobKeySerializer serializer = new();
        Assert.Throws<NotSupportedException>(() => serializer.Serialize(new { Tenant = 1, Job = 2 }));
    }
}

public sealed record TestJob(string JobId, DateTimeOffset Start, int Value) : IJob
{
    public object Id => JobId;
}

public sealed record TestQueueJob(
    string JobId,
    string Queue,
    DateTimeOffset Start,
    int Value) : IQueueJob
{
    public object Id => JobId;

    public object QueueId => Queue;
}

public sealed record TestRoutineJob(
    string JobId,
    DateTimeOffset Start,
    TimeSpan Interval) : IRoutineJob
{
    public object Id => JobId;
}

public sealed class TestJobHandler(JobProbe probe) : IJobHandler<TestJob>
{
    public Task HandleAsync(TestJob job, long tick, CancellationToken cancellationToken)
    {
        probe.Record(job.Value);
        return Task.CompletedTask;
    }
}

public sealed class TestQueueJobHandler(JobProbe probe) : IJobHandler<TestQueueJob>
{
    public Task HandleAsync(TestQueueJob job, long tick, CancellationToken cancellationToken)
    {
        probe.Record(job.Value);
        return Task.CompletedTask;
    }
}

public sealed class TestRoutineJobHandler(JobProbe probe) : IJobHandler<TestRoutineJob>
{
    public Task HandleAsync(TestRoutineJob job, long tick, CancellationToken cancellationToken)
    {
        probe.Record(checked((int)tick));
        return Task.CompletedTask;
    }
}

public sealed class JobProbe
{
    private readonly ConcurrentQueue<int> _values = new();
    private readonly SemaphoreSlim _available = new(0);

    public IReadOnlyCollection<int> Values => _values.ToArray();

    public void Record(int value)
    {
        _values.Enqueue(value);
        _available.Release();
    }

    public async Task<int> ReadAsync(CancellationToken cancellationToken)
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));
        await _available.WaitAsync(timeout.Token);
        Assert.True(_values.TryDequeue(out int value));
        return value;
    }
}
