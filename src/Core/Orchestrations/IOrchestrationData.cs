namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines the data that is passed between steps in an orchestration.
/// </summary>
public interface IOrchestrationData<out T>
{
    /// <summary>
    /// Gets the results of each step in the orchestration thus far.
    /// </summary>
    IReadOnlyList<IOrchestrationStepResult> Results { get; }

    /// <summary>
    /// Gets the <see cref="IOrchestrationBag"/> in use by the current orchestration.
    /// </summary>
    IOrchestrationBag Bag { get; }

    /// <summary>
    /// The orchestration that is being executed.
    /// </summary>
    T Orchestration { get; }

}
