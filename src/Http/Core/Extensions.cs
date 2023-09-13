using Ocluse.LiquidSnow.Cqrs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static QueryType GetQueryType(this IQueryRequest query)
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

        /// <summary>
        /// Checks for a property in a <see cref="JsonElement"/> without case sensitivity, and returns the value if found.
        /// </summary>
        public static bool TryGetPropertyNoCase(this JsonElement element, string propertyName, out JsonElement value)
        {
            value = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
