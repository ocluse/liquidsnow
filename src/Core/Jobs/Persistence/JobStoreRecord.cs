namespace Ocluse.LiquidSnow.Jobs.Persistence;

/// <summary>
/// The durable representation of a scheduled job.
/// </summary>
public sealed record JobStoreRecord
{
    /// <summary>Gets the serialized job identifier.</summary>
    public required string Id { get; init; }

    /// <summary>Gets the serialized queue identifier, or <see langword="null"/>.</summary>
    public string? QueueId { get; init; }

    /// <summary>Gets the registered stable job type name.</summary>
    public required string TypeName { get; init; }

    /// <summary>Gets the serialized job payload.</summary>
    public required byte[] Payload { get; init; }

    /// <summary>Gets the scheduling behavior.</summary>
    public JobScheduleKind ScheduleKind { get; init; }

    /// <summary>Gets the next time the job should run.</summary>
    public DateTimeOffset DueAt { get; init; }

    /// <summary>Gets the interval for a recurring job.</summary>
    public TimeSpan? Interval { get; init; }

    /// <summary>Gets the occurrence number passed to the handler.</summary>
    public long Tick { get; init; }

    /// <summary>Gets the durable insertion order.</summary>
    public long Sequence { get; init; }

    /// <summary>Gets the optimistic concurrency version.</summary>
    public long Version { get; init; }

    /// <summary>Gets the worker currently leasing the job.</summary>
    public string? LeaseOwner { get; init; }

    /// <summary>Gets the lease expiry time.</summary>
    public DateTimeOffset? LeaseExpiresAt { get; init; }

    /// <summary>Gets the number of times the job has been claimed.</summary>
    public int Attempts { get; init; }

    /// <summary>Gets the most recent execution error.</summary>
    public string? LastError { get; init; }
}
