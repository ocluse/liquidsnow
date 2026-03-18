namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class QueryDispatcher(CoreDispatcher coreDispatcher, IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await coreDispatcher.DispatchQueryAsync(query, serviceProvider, cancellationToken);
    }

    public Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQueryResult>
    {
        ArgumentNullException.ThrowIfNull(query);
        return coreDispatcher.DispatchQueryAsync<TQuery, TQueryResult>(query, serviceProvider, cancellationToken);
    }
} 
