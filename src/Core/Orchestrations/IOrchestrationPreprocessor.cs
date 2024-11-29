using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines a step that pre-processes an orchestration before it is orchestrated.
/// </summary>
/// <remarks>
///  It is useful for setting up the orchestration data among other preliminary tasks.
/// </remarks>
public interface IOrchestrationPreprocessor<in T, TResult>
    where T : IOrchestration<TResult>
{
    /// <summary>
    /// Execute the step and returns the result
    /// </summary>
    Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IOrchestrationPreprocessor{T, TResult}"/>
public interface IOrchestrationPreprocessor<in T> : IOrchestrationPreprocessor<T, Unit>
    where T : IOrchestration
{
}
