using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// The final step of an orchestration.
    /// </summary>
    /// <remarks>
    /// This step is expected to return the final result of the orchestration that is returned to the caller.
    /// </remarks>
    public interface IFinalOrchestrationStep<in T, TResult>
        where T : IOrchestration<TResult>
    {
        /// <summary>
        /// Executes the step.
        /// </summary>
        Task<TResult> Execute(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="IFinalOrchestrationStep{T, TResult}"/>
    public interface IFinalOrchestrationStep<in T> : IFinalOrchestrationStep<T, Unit>
        where T : IOrchestration
    {
    }
}
