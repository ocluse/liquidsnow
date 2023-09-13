namespace Ocluse.LiquidSnow.Http
{
    /// <summary>
    /// Represents the results of a query.
    /// </summary>
    /// <typeparam name="T">The type of returned results</typeparam>
    /// <typeparam name="Q">The type of query performed to produce this result</typeparam>
    public abstract class QueryResult<T, Q>
    {
        /// <summary>
        /// The type of query that was performed to produce this result.
        /// </summary>
        public abstract Q QType { get; }

        /// <summary>
        /// The items that matched the query that have been returned.
        /// </summary>
        public required IEnumerable<T> Items { get; init; }
    }

    ///<inheritdoc cref="QueryResult{T, Q}"/>
    public abstract class QueryResult<T> : QueryResult<T, QueryType>
    {
        
    }

    /// <summary>
    /// A query result that was paged using a cursor.
    /// </summary>
    public class CursorQueryResult<T> : QueryResult<T>
    {
        ///<inheritdoc/>
        public override QueryType QType => QueryType.Cursor;

        /// <summary>
        /// If specified, the cursor to use to retrieve the next page of results.
        /// </summary>
        public required string? Next { get; init; }

        /// <summary>
        /// If specified, the cursor to use to retrieve the previous page of results.
        /// </summary>
        public required string? Previous { get; init; }
    }

    /// <summary>
    /// A query result that was paged using an offset.
    /// </summary>
    public class OffsetQueryResult<T> : QueryResult<T>
    {
        ///<inheritdoc/>
        public override QueryType QType => QueryType.Offset;

        /// <summary>
        /// The number of items returned per page.
        /// </summary>
        public required int PageSize { get; init; }

        /// <summary>
        /// The current page number.
        /// </summary>
        public required int Page { get; init; }

        /// <summary>
        /// The total number of items that matched the query.
        /// </summary>
        public required int Total { get; init; }
    }

    /// <summary>
    /// A query result with items filtered using specified IDs
    /// </summary>
    public class IdsQueryResult<T> : QueryResult<T>
    {
        ///<inheritdoc/>
        public override QueryType QType => QueryType.Ids;
    }

    /// <summary>
    /// A query result with items filtered using a custom method
    /// </summary>
    public class CustomQueryResult<T> : QueryResult<T>
    {
        ///<inheritdoc/>
        public override QueryType QType => QueryType.Custom;
    }
}