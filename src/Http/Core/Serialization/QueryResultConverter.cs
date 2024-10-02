using System.Text.Json.Serialization;
using System.Text.Json;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Http.Serialization;

/// <summary>
/// Converts a <see cref="QueryResult{T}"/> to and from JSON.
/// </summary>
public class QueryResultConverter<T> : JsonConverter<QueryResult<T>>
{
    /// <summary>
    /// The type to use when deserializing a custom query result.
    /// </summary>
    protected virtual Type CustomQueryResultType { get; } = typeof(CustomQueryResult<T>);

    /// <summary>
    /// Returns the type matching the specified <see cref="QueryType"/>.
    /// </summary>
    protected virtual Type ResolveType(QueryType queryType)
    {
        return queryType switch
        {
            QueryType.Cursor => typeof(CursorQueryResult<T>),
            QueryType.Offset => typeof(OffsetQueryResult<T>),
            QueryType.Ids => typeof(IdsQueryResult<T>),
            QueryType.Custom => CustomQueryResultType,
            _ => throw new JsonException($"Unknown QueryType: {queryType}")
        };
    }

    ///<inheritdoc/>
    public override QueryResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        QueryResult<T>? result;

        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            if (doc.RootElement.TryGetPropertyNoCase(nameof(QueryResult<object>.QType), out var queryTypeProperty))
            {
                QueryType queryType = (QueryType)queryTypeProperty.GetInt32();

                Type type = ResolveType(queryType);

                result = (QueryResult<T>?)doc.Deserialize(type, options);
            }
            else
            {
                throw new JsonException($"{nameof(QueryType)} missing");
            }
        }
        else
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        return result;
    }

    ///<inheritdoc/>
    public override void Write(Utf8JsonWriter writer, QueryResult<T> value, JsonSerializerOptions options)
    {
        var type = value.GetType();
        JsonSerializer.Serialize(writer, value, type, options);
    }
}
