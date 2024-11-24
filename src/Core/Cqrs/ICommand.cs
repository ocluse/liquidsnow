using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Represents a request to change the system state, typically resulting in data modification.
/// </summary>
public interface ICommand<TCommandResult>
{
}

///<inheritdoc cref="ICommand{TCommandResult}"/>
public interface ICommand : ICommand<Unit>
{
}
