using System.Threading;
using System.Threading.Tasks;
using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// A contract for creating handlers for commands which are executed after the command is executed.
    /// </summary>
    public interface IPostCommandExecutionHandler<TCommand, TCommandResult>
    {
        /// <summary>
        /// Executes this handler.
        /// </summary>
        /// <remarks>
        /// The value returned by this handler will be deemed the final result of the command execution.
        /// </remarks>
        /// <param name="command">The command being executed</param>
        /// <param name="result">The result from the execution of the command</param>
        /// <param name="cancellationToken">The token used to cancel the operation</param>
        /// <returns></returns>
        Task<TCommandResult> Handle(TCommand command, TCommandResult result, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc cref="IPostCommandExecutionHandler{TCommand, TCommandResult}"/>
    public interface IPostCommandExecutionHandler<TCommand> : IPostCommandExecutionHandler<TCommand, Unit>
    {
    }
}
