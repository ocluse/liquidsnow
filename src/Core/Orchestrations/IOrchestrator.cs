namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines methods that orchestrates a series of steps.
/// </summary>
public interface IOrchestrator
{
    /// <summary>
    /// Execute an orchestration and returns the result.
    /// </summary>
    Task<T> ExecuteAsync<T>(IOrchestration<T> orchestration, CancellationToken cancellationToken = default);
}
