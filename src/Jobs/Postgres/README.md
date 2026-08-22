# LiquidSnow Jobs for PostgreSQL

This package persists LiquidSnow jobs in PostgreSQL and safely distributes due work between application instances using renewable leases and `FOR UPDATE SKIP LOCKED`.

## Configuration

```csharp
services
    .AddJobs()
    .AddJob<SendEmailJob>("email.send.v1")
    .UsePostgres(
        configuration.GetConnectionString("ApplicationDatabase")!,
        options => options.CreateSchemaIfNotExists = true);
```

`CreateSchemaIfNotExists` is intended for development and is disabled by default. For production deployments, execute the SQL returned by `PostgresJobSchema.GetCreateScript()` through the application's normal migration process.

An existing `NpgsqlDataSource` can be shared with EF Core:

```csharp
services
    .AddJobs()
    .AddJob<SendEmailJob>("email.send.v1")
    .UsePostgres(dataSource);
```

Use the asynchronous scheduler API when durability matters:

```csharp
await scheduler.ScheduleAsync(new SendEmailJob(
    Id: Guid.NewGuid(),
    Start: DateTimeOffset.UtcNow,
    MessageId: messageId));
```

Scheduling is at-least-once. Handlers should be idempotent because a worker can finish its external side effect and stop before recording completion.
