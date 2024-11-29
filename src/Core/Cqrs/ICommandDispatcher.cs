namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines methods responsible for routing commands to their designated handlers.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches the command to its designated handler and returns the result of the operation.
    /// </summary>
    Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);

    ///<inheritdoc cref="DispatchAsync{TCommandResult}(ICommand{TCommandResult}, CancellationToken)"/>
    Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommandResult>;
}
