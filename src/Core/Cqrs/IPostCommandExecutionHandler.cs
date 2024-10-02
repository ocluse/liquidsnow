using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// A contract for creating handlers that are executed after the command is executed by its main handler.
/// </summary>
public interface IPostCommandExecutionHandler<TCommand, TCommandResult>
{
    /// <summary>
    /// Executes this handler.
    /// </summary>
    Task<TCommandResult> HandleAsync(TCommand command, TCommandResult result, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IPostCommandExecutionHandler{TCommand, TCommandResult}"/>
public interface IPostCommandExecutionHandler<TCommand> : IPostCommandExecutionHandler<TCommand, Unit>
{
}
