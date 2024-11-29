namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that processes a specific type of query.
/// </summary>
public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
{
    /// <summary>
    /// Executes the query and returns the result.
    /// </summary>
    Task<TQueryResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}