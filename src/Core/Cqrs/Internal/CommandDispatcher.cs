namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class CommandDispatcher(CoreDispatcher coreDispatcher, IServiceProvider serviceProvider)
    : ICommandDispatcher
{
    public async Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await coreDispatcher.DispatchCommandAsync(command, serviceProvider, cancellationToken);
    }

    public Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<TCommandResult>
    {
        ArgumentNullException.ThrowIfNull(command);
        return coreDispatcher.DispatchCommandAsync<TCommand, TCommandResult>(command, serviceProvider, cancellationToken);
    }
}
