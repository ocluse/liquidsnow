using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines a step that is executed in an orchestration.
/// </summary>
public interface IOrchestrationStep<in T, TResult>
    where T : IOrchestration<TResult>
{
    /// <summary>
    /// Gets a value defining where the step is ordered in orchestration.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Executes the step and returns the result.
    /// </summary>
    Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<T> data, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IOrchestrationStep{T, TResult}"/>
public interface IOrchestrationStep<in T> : IOrchestrationStep<T, Unit>
    where T : IOrchestration
{
}
