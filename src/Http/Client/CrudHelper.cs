using Ocluse.LiquidSnow.Http.Cqrs;
using System.Text.Json;
using System.Web;

namespace Ocluse.LiquidSnow.Http.Client;

/// <summary>
/// Helper methods for CRUD request builders, to make it easier to perform queries.
/// </summary>
public static class CrudHelper
{
    /// <summary>
    /// The default page size to use when querying.
    /// </summary>
    public static int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Sends a request to query for a list of resources using offset paging.
    /// </summary>
    public static async Task<QueryResult<TResult>> QueryAsync<TKey, TQuery, TResult>(
        this IListRequestBuilder<TQuery, TResult> crudBuilder,
        int page,
        string? search = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
        where TQuery : ListQuery<TKey, TResult>
    {
        TQuery query = Activator.CreateInstance<TQuery>();
        query.Page = page;
        query.Search = search;
        query.Size = pageSize ?? DefaultPageSize;
        query.QType = QueryType.Offset;

        return await crudBuilder.ListAsync(query, cancellationToken);
    }

    /// <summary>
    /// Sends a request to query for a list of resources using cursor paging.
    /// </summary>
    public static async Task<QueryResult<TResult>> QueryAsync<TKey, TQuery, TResult>(
        this IListRequestBuilder<TQuery, TResult> crudBuilder,
        string? cursor,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
        where TQuery : ListQuery<TKey, TResult>
    {
        TQuery query = Activator.CreateInstance<TQuery>();
        query.Cursor = cursor;
        query.Size = pageSize ?? DefaultPageSize;
        query.QType = QueryType.Cursor;

        return await crudBuilder.ListAsync(query, cancellationToken);
    }

    /// <summary>
    /// Converts the given JSON string into a query string
    /// </summary>
    /// <remarks>
    /// For collections, they are flattened and added as separate parameters with the same key.
    /// </remarks>
    public static string ConvertJsonIntoQueryString(string json, JsonSerializerOptions? options = null)
    {
        var tuples = JsonSerializer.Deserialize<IDictionary<string, object>>(json, options)!
            .Where(x => x.Value != null)
            .Select(x => new Tuple<string, object>(x.Key, x.Value))
            .ToList();

        var collections = tuples.Where(x => x.Item2 is JsonElement y && y.ValueKind == JsonValueKind.Array).ToList();

        //remove all collections:
        foreach (var collection in collections)
        {
            tuples.Remove(collection);
        }

        //add back the collections as a list of new tuples:
        foreach (var collection in collections)
        {
            var enumerable = (JsonElement)collection.Item2;
            foreach (var item in enumerable.EnumerateArray())
            {
                tuples.Add(new Tuple<string, object>(collection.Item1, item));
            }
        }

        var step3 = tuples.Where(x => x.Item2 != null)
            .Select(x => HttpUtility.UrlEncode(x.Item1) + "=" + HttpUtility.UrlEncode(x.Item2.ToString()));

        return string.Join("&", step3);
    }
}
