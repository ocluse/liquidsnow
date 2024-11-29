namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class CommandDispatcher(CoreDispatcher coreDispatcher, IServiceProvider serviceProvider)
    : ICommandDispatcher
{
    public async Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken)
    {
        return await coreDispatcher.DispatchAsync<TCommandResult>(ExecutionKind.Command, command.GetType(),  command, serviceProvider, cancellationToken);
    }

    public Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TCommandResult>
    {
        return coreDispatcher.DispatchAsync<TCommandResult>(ExecutionKind.Command, typeof(TCommand), command, serviceProvider, cancellationToken);
    }
}
