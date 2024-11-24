namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Represents a <see cref="IJob"/> that can be handled by multiple handlers
/// </summary>
public interface IMulticastJob : IJob
{
    /// <summary>
    /// When true, all handlers for the job will be invoked in parallel.
    /// </summary>
    bool ExecuteParallel { get; }
}
