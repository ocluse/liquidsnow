using System.Text.Json;

namespace Ocluse.LiquidSnow.Http
{
    /// <summary>
    /// Adds extension methods to types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the most likely type of a query by looking at the properties of the query.
        /// </summary>
        public static QueryType GetQueryType<TKey>(this IQueryRequest<TKey> query)
        {
            if (query.QType != null)
            {
                return query.QType.Value;
            }
            else if (query.Ids != null && query.Ids.Any())
            {
                return QueryType.Ids;
            }
            else if (query.Page != null || !string.IsNullOrWhiteSpace(query.Search))
            {
                return QueryType.Offset;
            }
            return QueryType.Cursor;
        }

        /// <summary>
        /// Casts the query as a <see cref="OffsetQueryResult{T}"/>
        /// </summary>
        /// <returns></returns>
        public static OffsetQueryResult<T> AsOffset<T>(this QueryResult<T> query)
        {
            return (OffsetQueryResult<T>)query;
        }

        /// <summary>
        /// Casts the query as a <see cref="CursorQueryResult{T}"/>
        /// </summary>
        public static CursorQueryResult<T> AsCursor<T>(this QueryResult<T> query)
        {
            return (CursorQueryResult<T>)query;
        }

        /// <summary>
        /// Casts the query as a <see cref="IdsQueryResult{T}"/>
        /// </summary>
        public static IdsQueryResult<T> AsIds<T>(this QueryResult<T> query)
        {
            return (IdsQueryResult<T>)query;
        }
    }
}
