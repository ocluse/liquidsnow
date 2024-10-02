﻿namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// A contract for creating handlers for queries which are executed before the query is executed by its main handler.
/// </summary>
public interface IPreQueryExecutionHandler<TQuery, TQueryResult>
{
    /// <summary>
    /// Executes this handler.
    /// </summary>
    /// <remarks>
    /// If the handler returns a continue response, the query will be executed.
    /// Otherwise, the query will not be executed and the value returned by this handler will be deemed the result.
    /// </remarks>
    Task<PreExecutionResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
