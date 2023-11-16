namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// A contract for creating handlers for queries which are executed after the query is executed.
    /// </summary>
    public interface IPostQueryExecutionHandler<TQuery, TQueryResult>
    {
        /// <summary>
        /// Executes this handler.
        /// </summary>
        /// <remarks>
        /// The value returned by this handler will be deemed the final result of the query execution.
        /// </remarks>
        /// <param name="query">The query being executed</param>
        /// <param name="result">The result from the execution of the query</param>
        /// <param name="cancellationToken">The token used to cancel the operation</param>
        Task<TQueryResult> Handle(TQuery query, TQueryResult result, CancellationToken cancellationToken = default);
    }
}
