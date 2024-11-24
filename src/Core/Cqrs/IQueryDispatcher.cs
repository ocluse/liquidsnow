namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines methods responsible for routing queries to their designated handlers.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches the query to its appropriate handler and returns the result of the operation.
    /// </summary>
    Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);

    ///<inheritdoc cref="DispatchAsync{TQueryResult}(IQuery{TQueryResult}, CancellationToken)"/>
    Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken = default)
     where TQuery : IQuery<TQueryResult>;
}