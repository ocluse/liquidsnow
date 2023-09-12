using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Cqrs.Internal
{
    internal class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<TQueryResult> Dispatch<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken)
        {
            ExecutionDescriptor descriptor = ExecutionsHelper.GetDescriptor<TQueryResult>(ExecutionKind.Query, query);

            return await ExecutionsHelper.ExecuteDescriptor<TQueryResult>(query, descriptor, _serviceProvider, cancellationToken);
        }
    }
}