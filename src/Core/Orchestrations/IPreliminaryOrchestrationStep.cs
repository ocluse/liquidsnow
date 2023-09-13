using System.Threading.Tasks;
using System.Threading;
using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// The first step of an orchestration.
    /// </summary>
    /// <remarks>
    ///  This step is always executed first despite the order of all other steps.
    ///  It is useful for setting up the orchestration data among other preliminary tasks.
    /// </remarks>
    public interface IPreliminaryOrchestrationStep<in T, TResult>
        where T : IOrchestration<TResult>
    {
        /// <summary>
        /// Execute the step.
        /// </summary>
        Task<IOrchestrationStepResult> Execute(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="IPreliminaryOrchestrationStep{T, TResult}"/>
    public interface IPreliminaryOrchestrationStep<in T> : IPreliminaryOrchestrationStep<T, Unit>
        where T : IOrchestration
    {
    }
}
