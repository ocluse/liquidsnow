using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that pre-processes a specific type of command before it is executed by its main handler.
/// </summary>
public interface ICommandPreprocessor<TCommand, TCommandResult> : ICommand<TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    /// <summary>
    /// Pre processes the command.
    /// </summary>
    Task<TCommand> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="ICommandPreprocessor{TCommand, TCommandResult}"/>
public interface ICommandPreprocessor<TCommand> : ICommandPreprocessor<TCommand, Unit>
    where TCommand : ICommand
{
}