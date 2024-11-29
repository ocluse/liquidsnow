namespace Ocluse.LiquidSnow.Orchestrations;

/// <inheritdoc cref="IFinalOrchestrationResult{T}"/>
public readonly struct FinalOrchestrationResult<T>(T data, bool isSuccess)
    : IFinalOrchestrationResult<T>
{
    ///<inheritdoc/>
    public readonly bool IsSuccess { get; } = isSuccess;

    ///<inheritdoc cref="IFinalOrchestrationResult{T}.Data"/>
    public readonly T Data { get; } = data;

    ///<inheritdoc/>
    public readonly int? GoToOrder => null;
}
