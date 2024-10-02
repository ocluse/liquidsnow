using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// A contract for creating handlers for commands which are executed before the command is executed by its main handler.
/// </summary>
public interface IPreCommandExecutionHandler<TCommand, TCommandResult>
{
    /// <summary>
    /// Executes this handler.
    /// </summary>
    /// <remarks>
    /// If the handler returns a continue response, the command will be executed.
    /// Otherwise, the command will not be executed and the value returned by this handler will be deemed the result.
    /// </remarks>
    Task<PreExecutionResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IPreCommandExecutionHandler{TCommand, TCommandResult}"/>
public interface IPreCommandExecutionHandler<TCommand> : IPreCommandExecutionHandler<TCommand, Unit>
{
}
