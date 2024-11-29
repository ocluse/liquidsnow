using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations;


///<inheritdoc cref="IOrchestrationStepResult"/>
public readonly struct OrchestrationStepResult(int? jumpToStep) : IOrchestrationStepResult
{
    ///<inheritdoc cref="IOrchestrationStepResult.GoToOrder"/>
    public int? GoToOrder { get; } = jumpToStep;

    /// <summary>
    /// Returns a result indicating the orchestration should continue to the next step.
    /// </summary>
    public static IOrchestrationStepResult Next()
    {
        return new OrchestrationStepResult(null);
    }

    /// <summary>
    /// Returns a result indicating the orchestration should jump to the specified step.
    /// </summary>
    public static IOrchestrationStepResult GoTo(int goToOrder)
    {
        return new OrchestrationStepResult(goToOrder);
    }

    /// <summary>
    /// Returns a result that will cause the orchestration to finalize and return the specified data.
    /// </summary>
    public static IFinalOrchestrationResult<T> Break<T>(T data, bool isSuccess = true)
    {
        return new FinalOrchestrationResult<T>(data, isSuccess);
    }

    /// <summary>
    /// Returns a result that will cause the orchestration to finalize.
    /// </summary>
    /// <remarks>
    /// This is a convenience method for when the orchestration is not returning any data, 
    /// and it is typically used with the non-generic <see cref="IOrchestration"/> interface.
    /// </remarks>
    public static IFinalOrchestrationResult<Unit> Break(bool isSuccess = true)
    {
        return new FinalOrchestrationResult<Unit>(default, isSuccess);
    }
}
