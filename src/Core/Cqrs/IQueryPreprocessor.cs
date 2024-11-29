namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that pre processes a specific type of query before it is executed by its main handler.
/// </summary>
public interface IQueryPreprocessor<TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
{
    /// <summary>
    /// Pre processes the query.
    /// </summary>
    Task<TQuery> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
