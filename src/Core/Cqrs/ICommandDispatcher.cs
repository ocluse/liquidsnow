namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// Utility methods to dispatch commands
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches the command to its appropriate handler.
        /// </summary>
        /// <typeparam name="TCommandResult">The expected result of the operation</typeparam>
        /// <param name="command">The command to be executed</param>
        /// <param name="cancellationToken">The token used to cancel the command execution</param>
        /// <returns>The result of the execution of the command</returns>
        Task<TCommandResult> Dispatch<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);
    }
}
