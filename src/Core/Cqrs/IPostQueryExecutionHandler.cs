namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// A contract for creating handlers for queries which are executed after the query is executed by its main handler.
/// </summary>
public interface IPostQueryExecutionHandler<TQuery, TQueryResult>
{
    /// <summary>
    /// Executes the handler and returns the result of the operation.
    /// </summary>
    /// <remarks>
    /// The value returned by this handler will be deemed the final result of the query execution.
    /// </remarks>
    Task<TQueryResult> HandleAsync(TQuery query, TQueryResult result, CancellationToken cancellationToken = default);
}
