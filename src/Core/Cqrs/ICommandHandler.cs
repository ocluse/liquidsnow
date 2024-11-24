using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that processes a specific type of command.
/// </summary>
public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    /// <summary>
    /// Executes the command, returning the result.
    /// </summary>
    Task<TCommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="ICommandHandler{TCommand, TCommandResult}"/>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand
{

}
