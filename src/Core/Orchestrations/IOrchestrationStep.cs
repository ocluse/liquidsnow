using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// A step that is executed in an orchestration.
    /// </summary>
    public interface IOrchestrationStep<in T, TResult>
        where T : IOrchestration<TResult>
    {
        /// <summary>
        /// The order in which the step is executed.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Execute the step.
        /// </summary>
        Task<IOrchestrationStepResult> Execute(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="IOrchestrationStep{T, TResult}"/>
    public interface IOrchestrationStep<in T> : IOrchestrationStep<T, Unit>
        where T : IOrchestration
    {
    }
}
