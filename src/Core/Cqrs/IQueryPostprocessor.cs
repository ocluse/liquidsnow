namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that post processes a specific type of query after it has been executed by its main handler.
/// </summary>
public interface IQueryPostprocessor<TQuery, TQueryResult>
{
    /// <summary>
    /// Post processes the query, returning a result that overrides the original result.
    /// </summary>
    Task<TQueryResult> HandleAsync(TQuery query, TQueryResult result, CancellationToken cancellationToken = default);
}
