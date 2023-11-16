namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// Utility methods to dispatch queries
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatches the query to its appropriate handler
        /// </summary>
        /// <typeparam name="TQueryResult">The expected result of the operation</typeparam>
        /// <param name="query">The query to be executed</param>
        /// <param name="cancellationToken">The token used to cancel the execution</param>
        /// <returns>The result of the execution of the query</returns>
        Task<TQueryResult> Dispatch<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}