namespace Ocluse.LiquidSnow.Jobs.Persistence;

/// <summary>
/// Parameters used when claiming due jobs.
/// </summary>
public sealed record JobClaimRequest(
    DateTimeOffset Now,
    string WorkerId,
    TimeSpan LeaseDuration,
    int MaximumCount);

/// <summary>
/// Identifies a claimed job using its lease and optimistic concurrency version.
/// </summary>
public sealed record JobLease(
    string Id,
    string? QueueId,
    string WorkerId,
    long Version);

/// <summary>
/// Describes the durable transition following an execution attempt.
/// </summary>
public sealed record JobCompletion(
    JobLease Lease,
    DateTimeOffset? NextDueAt,
    long NextTick,
    string? Error);
