namespace Ocluse.LiquidSnow.Http
{
    /// <summary>
    /// Represents a query made to a service to retrieve data.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the query</typeparam>
    /// <typeparam name="Q">The type of the query to perform</typeparam>
    public interface IQueryRequest<TKey, Q>
    {
        /// <summary>
        /// If specified, the query will be paged using the cursor.
        /// </summary>
        string? Cursor { get; set; }

        /// <summary>
        /// If specified, the query should return only the items with the specified ids.
        /// </summary>
        IEnumerable<TKey>? Ids { get; set; }

        /// <summary>
        /// If specified, the query will be paged using the offset.
        /// </summary>
        int? Page { get; set; }

        /// <summary>
        /// Used to determine the type of query to perform
        /// </summary>
        Q? QType { get; set; }

        /// <summary>
        /// If specified, the query items will be filtered using the specified search string.
        /// </summary>
        string? Search { get; set; }

        /// <summary>
        /// If specified, the returned items will be limited to the specified size.
        /// </summary>
        int? Size { get; set; }
    }

    ///<inheritdoc cref="IQueryRequest{TKey, Q}"/>
    public interface IQueryRequest<TKey> : IQueryRequest<TKey, QueryType?>
    {

    }
}