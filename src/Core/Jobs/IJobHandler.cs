namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Defines a handler that processes a specific type of job.
/// </summary>
public interface IJobHandler<in T> where T : IJob
{
    /// <summary>
    /// Executes the provided job.
    /// </summary>
    Task Handle(T job, long tick, CancellationToken cancellationToken);
}
