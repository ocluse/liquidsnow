using System.Threading.Tasks;
using System.Threading;
using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// An orchestration step that can be skipped based on the result of a condition.
    /// </summary>
    public interface IConditionalOrchestrationStep<in T, TResult> : IOrchestrationStep<T, TResult>
        where T : IOrchestration<TResult>
    {
        /// <summary>
        /// Check if the step can be executed.
        /// </summary>
        Task<bool> CanExecute(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="IConditionalOrchestrationStep{T, TResult}"/>
    public interface IConditionalOrchestrationStep<in T> : IConditionalOrchestrationStep<T, Unit>
        where T : IOrchestration
    {
    }
}
