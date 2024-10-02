namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Utility methods to dispatch commands to their appropriate handlers.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Dispatches the command to its appropriate handler and returns the result of the operation.
    /// </summary>
    Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);
}
