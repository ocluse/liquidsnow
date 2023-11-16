namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// An orchestrator that executes an orchestration.
    /// </summary>
    public interface IOrchestrator
    {
        /// <summary>
        /// Execute an orchestration.
        /// </summary>
        Task<T> Execute<T>(IOrchestration<T> orchestration, CancellationToken cancellationToken = default);
    }
}
