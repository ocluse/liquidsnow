namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task<TCommandResult> DispatchAsync<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken)
    {
        ExecutionDescriptor descriptor = ExecutionsHelper.GetDescriptor<TCommandResult>(ExecutionKind.Command, command);

        return await ExecutionsHelper.ExecuteDescriptorAsync<TCommandResult>(command, descriptor, serviceProvider, cancellationToken);
    }
}
