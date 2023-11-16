using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// Provides a contract for creating handlers for commands, which will execute the command
    /// </summary>
    /// <typeparam name="TCommand">The type of cammand the handler accepts</typeparam>
    /// <typeparam name="TCommandResult">The type of result the handler outputs after handling the command</typeparam>
    public interface ICommandHandler<in TCommand, TCommandResult> where TCommand : ICommand<TCommandResult>
    {
        /// <summary>
        /// Handles/Executes the provided command.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="cancellationToken">The token used to cancel the execution</param>
        /// <returns>The result of the command's execution</returns>
        Task<TCommandResult> Handle(TCommand command, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="ICommandHandler{TCommand, TCommandResult}"/>
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand
    {

    }
}
