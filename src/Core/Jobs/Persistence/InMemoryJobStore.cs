namespace Ocluse.LiquidSnow.Jobs.Persistence;

/// <summary>
/// Process-local job store used when no durable provider is configured.
/// </summary>
public sealed class InMemoryJobStore : IJobStore
{
    private readonly object _sync = new();
    private readonly Dictionary<(string Scope, string Id), JobStoreRecord> _jobs = [];
    private long _nextSequence;

    /// <inheritdoc />
    public ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask StoreAsync(JobStoreRecord job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            (string Scope, string Id) key = GetKey(job.Id, job.QueueId);
            long version = _jobs.TryGetValue(key, out JobStoreRecord? existing)
                ? existing.Version + 1
                : 0;

            _jobs[key] = job with
            {
                Sequence = ++_nextSequence,
                Version = version,
                LeaseOwner = null,
                LeaseExpiresAt = null,
                Attempts = 0,
                LastError = null
            };
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<bool> CancelAsync(
        string id,
        string? queueId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            return ValueTask.FromResult(_jobs.Remove(GetKey(id, queueId)));
        }
    }

    /// <inheritdoc />
    public ValueTask<IReadOnlyList<JobStoreRecord>> ClaimDueAsync(
        JobClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            HashSet<string> blockedQueues = _jobs.Values
                .Where(job => job.QueueId is not null && IsActivelyLeased(job, request.Now))
                .Select(job => job.QueueId!)
                .ToHashSet(StringComparer.Ordinal);

            IEnumerable<JobStoreRecord> nonQueued = _jobs.Values
                .Where(job => job.QueueId is null && IsClaimable(job, request.Now));

            IEnumerable<JobStoreRecord> queued = _jobs.Values
                .Where(job => job.QueueId is not null && !blockedQueues.Contains(job.QueueId) && IsClaimable(job, request.Now))
                .GroupBy(job => job.QueueId, StringComparer.Ordinal)
                .Select(group => group.OrderBy(job => job.DueAt).ThenBy(job => job.Sequence).First());

            JobStoreRecord[] selected = nonQueued
                .Concat(queued)
                .OrderBy(job => job.DueAt)
                .ThenBy(job => job.Sequence)
                .Take(request.MaximumCount)
                .ToArray();

            List<JobStoreRecord> claimed = new(selected.Length);
            foreach (JobStoreRecord job in selected)
            {
                JobStoreRecord updated = job with
                {
                    LeaseOwner = request.WorkerId,
                    LeaseExpiresAt = request.Now + request.LeaseDuration,
                    Attempts = job.Attempts + 1,
                    Version = job.Version + 1
                };

                _jobs[GetKey(job.Id, job.QueueId)] = updated;
                claimed.Add(updated);
            }

            return ValueTask.FromResult<IReadOnlyList<JobStoreRecord>>(claimed);
        }
    }

    /// <inheritdoc />
    public ValueTask<bool> RenewLeaseAsync(
        JobLease lease,
        DateTimeOffset leaseExpiresAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lease);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            (string Scope, string Id) key = GetKey(lease.Id, lease.QueueId);
            if (!_jobs.TryGetValue(key, out JobStoreRecord? job) || !OwnsLease(job, lease))
            {
                return ValueTask.FromResult(false);
            }

            _jobs[key] = job with { LeaseExpiresAt = leaseExpiresAt };
            return ValueTask.FromResult(true);
        }
    }

    /// <inheritdoc />
    public ValueTask<bool> CompleteAsync(
        JobCompletion completion,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(completion);
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            (string Scope, string Id) key = GetKey(completion.Lease.Id, completion.Lease.QueueId);
            if (!_jobs.TryGetValue(key, out JobStoreRecord? job) || !OwnsLease(job, completion.Lease))
            {
                return ValueTask.FromResult(false);
            }

            if (completion.NextDueAt is null)
            {
                _jobs.Remove(key);
            }
            else
            {
                _jobs[key] = job with
                {
                    DueAt = completion.NextDueAt.Value,
                    Tick = completion.NextTick,
                    Version = job.Version + 1,
                    LeaseOwner = null,
                    LeaseExpiresAt = null,
                    LastError = completion.Error
                };
            }

            return ValueTask.FromResult(true);
        }
    }

    /// <inheritdoc />
    public ValueTask<DateTimeOffset?> GetNextDueTimeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_sync)
        {
            DateTimeOffset? next = _jobs.Values
                .Select(job => job.LeaseOwner is null ? job.DueAt : job.LeaseExpiresAt)
                .Where(time => time is not null)
                .Min();
            return ValueTask.FromResult(next);
        }
    }

    private static bool IsClaimable(JobStoreRecord job, DateTimeOffset now)
    {
        return job.LeaseOwner is null
            ? job.DueAt <= now
            : job.LeaseExpiresAt <= now;
    }

    private static bool IsActivelyLeased(JobStoreRecord job, DateTimeOffset now)
    {
        return job.LeaseOwner is not null && job.LeaseExpiresAt > now;
    }

    private static bool OwnsLease(JobStoreRecord job, JobLease lease)
    {
        return job.Version == lease.Version
            && string.Equals(job.LeaseOwner, lease.WorkerId, StringComparison.Ordinal);
    }

    private static (string Scope, string Id) GetKey(string id, string? queueId)
    {
        return (queueId ?? string.Empty, id);
    }
}
