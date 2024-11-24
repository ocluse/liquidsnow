namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Represents the result of an orchestration step.
/// </summary>
public interface IOrchestrationStepResult
{
    /// <summary>
    /// Gets a value, that if not null will cause the orchestration to jump to the step with the specified order.
    /// </summary>
    int? GoToOrder { get; }
}

/// <summary>
/// A result indicating that the orchestration should be halted.
/// </summary>
/// <remarks>
/// The data returned by this result will be the final result of the orchestration.
/// </remarks>
public interface IFinalOrchestrationResult<out T> : IOrchestrationStepResult
{
    /// <summary>
    /// Gets the data that is returned by the step.
    /// </summary>
    T Data { get; }
}
