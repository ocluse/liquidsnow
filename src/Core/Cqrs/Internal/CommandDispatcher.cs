using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Cqrs.Internal
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<TCommandResult> Dispatch<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken)
        {
            ExecutionDescriptor descriptor = ExecutionsHelper.GetDescriptor<TCommandResult>(ExecutionKind.Command, command);

            return await ExecutionsHelper.ExecuteDescriptor<TCommandResult>(command, descriptor, _serviceProvider, cancellationToken);
        }
    }
}
