using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines a step that post-processes an orchestration, returning the result of the orchestration.
/// </summary>
public interface IOrchestrationPostprocessor<in T, TResult>
    where T : IOrchestration<TResult>
{
    /// <summary>
    /// Executes the step and returns the result.
    /// </summary>
    Task<TResult> ExecuteAsync(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IOrchestrationPostprocessor{T, TResult}"/>
public interface IOrchestrationPostprocessor<in T> : IOrchestrationPostprocessor<T, Unit>
    where T : IOrchestration
{
}
