namespace Ocluse.LiquidSnow.Jobs.Persistence;

/// <summary>
/// Defines durable operations used by the job scheduler.
/// </summary>
public interface IJobStore
{
    /// <summary>
    /// Initializes the store before it is used by a worker.
    /// </summary>
    ValueTask InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or replaces a job.
    /// </summary>
    ValueTask StoreAsync(JobStoreRecord job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a job if it exists.
    /// </summary>
    ValueTask<bool> CancelAsync(string id, string? queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically leases jobs that are ready to execute.
    /// </summary>
    ValueTask<IReadOnlyList<JobStoreRecord>> ClaimDueAsync(
        JobClaimRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renews an active lease without changing its concurrency version.
    /// </summary>
    ValueTask<bool> RenewLeaseAsync(
        JobLease lease,
        DateTimeOffset leaseExpiresAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes an execution and either removes or reschedules the job.
    /// </summary>
    ValueTask<bool> CompleteAsync(JobCompletion completion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next time at which the store may contain claimable work.
    /// </summary>
    ValueTask<DateTimeOffset?> GetNextDueTimeAsync(CancellationToken cancellationToken = default);
}
