namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Utility methods to dispatch queries.
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Dispatches the query to its appropriate handler and returns the result of the operation.
    /// </summary>
    Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
}