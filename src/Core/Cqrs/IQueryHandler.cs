namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// Provides a contract for creating handlers for queries.
    /// </summary>
    /// <typeparam name="TQuery">The type of query the handler accepts</typeparam>
    /// <typeparam name="TQueryResult">The type of result the handler outputs after executing the query</typeparam>
    public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
    {
        /// <summary>
        /// Runs the provided query
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <param name="cancellationToken">The token used to cancel the execution</param>
        /// <returns>The result of the query's exectuion</returns>
        Task<TQueryResult> Handle(TQuery query, CancellationToken cancellationToken = default);
    }
}