namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Provides a contract for creating handlers for queries.
/// </summary>
public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
{
    /// <summary>
    /// Runs the query and returns the result of the operation.
    /// </summary>
    Task<TQueryResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}