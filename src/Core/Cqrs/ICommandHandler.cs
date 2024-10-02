using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Provides a contract for creating handlers for commands, which will execute the command.
/// </summary>
public interface ICommandHandler<in TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>
{
    /// <summary>
    /// Executes the command and returns the result.
    /// </summary>
    Task<TCommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="ICommandHandler{TCommand, TCommandResult}"/>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand
{

}
