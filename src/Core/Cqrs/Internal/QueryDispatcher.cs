namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken)
    {
        ExecutionDescriptor descriptor = ExecutionsHelper.GetDescriptor<TQueryResult>(ExecutionKind.Query, query);

        return await ExecutionsHelper.ExecuteDescriptorAsync<TQueryResult>(query, descriptor, serviceProvider, cancellationToken);
    }
}