using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Represents an action that is executed as an orchestration of steps.
/// </summary>
public interface IOrchestration<T>
{

}

///<inheritdoc cref="IOrchestration{T}"/>
public interface IOrchestration : IOrchestration<Unit>
{

}
