# Jobs: application guide

LiquidSnow Jobs schedules .NET objects for execution by dependency-injected handlers. Jobs may run once, on a recurring schedule, or serially within a named queue. The default store is process-local; the PostgreSQL provider makes schedules durable and allows several application instances to share the work safely.

## Define a job and handler

A job implements `IJob`. Its `Id` identifies the schedule and its `Start` value is the first time it becomes eligible to run.

```csharp
using Ocluse.LiquidSnow.Jobs;

public sealed record SendEmailJob(
    Guid MessageId,
    DateTimeOffset Start) : IJob
{
    public object Id => MessageId;
}

public sealed class SendEmailJobHandler(IEmailSender emailSender)
    : IJobHandler<SendEmailJob>
{
    public Task HandleAsync(
        SendEmailJob job,
        long tick,
        CancellationToken cancellationToken)
    {
        return emailSender.SendAsync(job.MessageId, cancellationToken);
    }
}
```

Handlers are resolved from a new dependency-injection scope for each execution, so a handler may depend on scoped services. The `tick` starts at `0` and increments after every completed occurrence of a recurring job. It is always `0` for a one-time job.

## Register the job system

Register handlers from the assembly that contains them. Assign an explicit, stable serialization name to every durable job type:

```csharp
using Ocluse.LiquidSnow.DependencyInjection;

builder.Services
    .AddJobs(typeof(SendEmailJobHandler).Assembly)
    .AddJob<SendEmailJob>("email.send.v1")
    .Configure(options =>
    {
        options.MaximumConcurrency = 8;
        options.ClaimBatchSize = 16;
        options.PollInterval = TimeSpan.FromSeconds(2);
        options.LeaseDuration = TimeSpan.FromMinutes(1);
    });
```

`AddJobs()` without an assembly scans its calling assembly. Scanning registers each `IJobHandler<T>` and also registers `T` under its CLR full name. An explicit `AddJob<T>(name)` is preferable for persisted jobs because CLR namespaces and type names commonly change during refactoring. Keep old names registered or migrate stored records when renaming a durable type.

The scheduler is also registered as an `IHostedService`. A generic host starts it and recovers existing work automatically. Resolving `IJobScheduler` without a generic host is supported, and scheduling a new job starts its worker, but automatic startup recovery requires the hosted-service lifecycle.

## Schedule and cancel work

Use the asynchronous API when the caller must know that the store accepted the job:

```csharp
IJobScheduler scheduler = services.GetRequiredService<IJobScheduler>();

IJobHandle handle = await scheduler.ScheduleAsync(
    new SendEmailJob(messageId, DateTimeOffset.UtcNow),
    cancellationToken);
```

`ScheduleAsync` stores the job before it returns. `Schedule` is its synchronous wrapper. A job whose `Start` is in the past becomes eligible on the worker's next claim.

Cancel through the handle or by the original typed identifier:

```csharp
bool cancelledByHandle = await handle.CancelAsync(cancellationToken);
bool cancelledById = await scheduler.CancelAsync(messageId, cancellationToken);
```

Disposing an `IJobHandle` also cancels its job. Do not put a handle in a short-lived `using` block if the schedule should survive the method that created it. Cancellation removes pending work and signals the cancellation token of an execution active in the same process. Handlers must observe that token; cancellation cannot undo side effects already performed.

Scheduling the same serialized ID again replaces the existing non-queued job. For queued work, the identity is the pair `(QueueId, Id)`, so the same job ID may exist in different queues.

The default key serializer supports `string`, `Guid`, integral numeric types, and enums. Their type prefixes make, for example, the string `"42"` distinct from the integer `42`. Register a custom `IJobKeySerializer` if IDs are compound values.

## Queue jobs

Implement `IQueueJob` when jobs sharing a queue must never overlap:

```csharp
public sealed record ImportCustomerJob(
    Guid ImportId,
    Guid CustomerId,
    DateTimeOffset Start) : IQueueJob
{
    public object Id => ImportId;
    public object QueueId => CustomerId;
}

await scheduler.QueueAsync(
    new ImportCustomerJob(importId, customerId, DateTimeOffset.UtcNow),
    cancellationToken);
```

Within one queue, only one due job is leased at a time. Eligible jobs are ordered by `Start` and then durable insertion order. Different queues and ordinary jobs may execute concurrently, subject to `MaximumConcurrency`.

Cancel queued work with its queue and job IDs, or use the returned handle:

```csharp
await scheduler.CancelAsync(customerId, importId, cancellationToken);
```

## Recurring jobs

An `IRoutineJob` uses a fixed-rate schedule. Each next run is calculated from the previously scheduled time. When execution runs late, missed occurrences are skipped rather than replayed:

```csharp
public sealed record RefreshCacheJob(
    string CacheName,
    DateTimeOffset Start,
    TimeSpan Interval) : IRoutineJob
{
    public object Id => CacheName;
}
```

An `ITaskSeriesJob` uses a fixed delay instead: its next run is scheduled for one interval after the preceding handler finishes.

```csharp
public sealed record PollPartnerJob(
    string Partner,
    DateTimeOffset Start,
    TimeSpan Interval) : ITaskSeriesJob
{
    public object Id => Partner;
}
```

Intervals must be greater than zero. Both recurring variants remain in the store until explicitly cancelled.

## Multicast and polymorphic handlers

An ordinary job resolves one `IJobHandler<T>`. An `IMulticastJob` resolves every registered handler for its type. Set `ExecuteParallel` to run those handlers concurrently; otherwise registration order is used.

By default, dispatch requires a handler for the concrete runtime type. Set `JobsOptions.EnablePolymorphicResolution` to allow fallback through base classes, or put `[PolymorphicResolution]` on selected job classes. Interfaces are not part of this fallback chain. Multicast jobs invoke handlers found along the enabled concrete-to-base-class chain.

## Failures and delivery guarantees

The durable scheduler provides **at-least-once execution**, not exactly-once execution. If a process stops after a handler performs an external side effect but before completion is stored, another worker can recover the expired lease and execute the job again. Make handlers idempotent, typically by using the job ID as an idempotency key or recording completion in the same transactional system as the side effect.

Handler exceptions publish `JobFailedEvent` when an event bus or listeners are registered. The scheduler records the exception text as the last error for recurring work. A handler exception is considered a completed attempt: a one-time job is removed, and a recurring job advances to its next occurrence. Built-in automatic retries are only a consequence of interrupted execution or lease recovery; there is no handler-exception retry policy.

If no matching handler is registered, dispatch currently completes without error. Ensure every persisted job type and handler is registered in every worker deployment.

## PostgreSQL persistence

Reference `Ocluse.LiquidSnow.Jobs.Postgres` and select it while registering jobs:

```csharp
builder.Services
    .AddJobs(typeof(SendEmailJobHandler).Assembly)
    .AddJob<SendEmailJob>("email.send.v1")
    .UsePostgres(
        builder.Configuration.GetConnectionString("ApplicationDatabase")!,
        options =>
        {
            options.Schema = "liquidsnow_jobs";
            options.CreateSchemaIfNotExists = true;
        });
```

`CreateSchemaIfNotExists` defaults to `false`. It is convenient during development; production deployments should execute `PostgresJobSchema.GetCreateScript(schema)` through their normal migration process. Schema names may contain only letters, digits, and underscores and cannot start with a digit.

An existing `NpgsqlDataSource` can be shared instead of giving the provider a connection string:

```csharp
builder.Services
    .AddJobs(typeof(SendEmailJobHandler).Assembly)
    .AddJob<SendEmailJob>("email.send.v1")
    .UsePostgres(dataSource);
```

All instances that share the table may poll it. PostgreSQL row locks, leases, and optimistic versions ensure that only one instance owns a particular execution at a time, while expired leases make interrupted jobs available again.

## Configuration reference

| Option | Default | Purpose |
| --- | --- | --- |
| `EnablePolymorphicResolution` | `false` | Fall back from a concrete job type through its base classes. |
| `SerializerOptions` | `JsonSerializerDefaults.Web` | Configure JSON payload serialization. Configure this collection during job registration. |
| `PollInterval` | 5 seconds | Maximum idle delay before checking the store again. New jobs scheduled in the same process wake the worker immediately. |
| `LeaseDuration` | 1 minute | Ownership period for a claimed job. A running worker renews it every one third of this duration. |
| `ClaimBatchSize` | 32 | Maximum records requested in one claim, also limited by available execution slots. |
| `MaximumConcurrency` | processor count, at least 1 | Maximum handlers running concurrently in one application instance. |

All durations and numeric limits must be greater than zero.

