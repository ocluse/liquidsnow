using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// An orchestration step that can be skipped based on the result of a previous execution step.
    /// </summary>
    /// <remarks>
    ///  If this is the very first step of the orchestration, the step will always be skipped.
    ///  This is because there is no data from a previous execution to check against.
    /// </remarks>
    public interface IStateDependentOrchestrationStep<in T, TResult> : IOrchestrationStep<T, TResult>
        where T : IOrchestration<TResult>
    {
        /// <summary>
        /// The previous execution state that the step depends on, without which the step will be skipped.
        /// </summary>
        RequiredState RequiredState { get; }
    }

    ///<inheritdoc cref="IStateDependentOrchestrationStep{T, TResult}"/>
    public interface IStateDependentOrchestrationStep<in T> : IStateDependentOrchestrationStep<T, Unit>
        where T : IOrchestration
    {
    }
}
