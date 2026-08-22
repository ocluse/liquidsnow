namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Represents a durable handle to a scheduled job.
/// </summary>
public interface IJobHandle : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the serialized identity of the job.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the serialized queue identity, or <see langword="null"/> for a non-queued job.
    /// </summary>
    string? QueueId { get; }

    /// <summary>
    /// Cancels the job if it is still present in the job store.
    /// </summary>
    ValueTask<bool> CancelAsync(CancellationToken cancellationToken = default);
}
