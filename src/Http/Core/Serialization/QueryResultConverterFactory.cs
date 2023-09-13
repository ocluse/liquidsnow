using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ocluse.LiquidSnow.Http.Serialization
{
    /// <summary>
    /// A factory for creating <see cref="QueryResultConverter{T}"/> instances.
    /// </summary>
    public class QueryResultConverterFactory : JsonConverterFactory
    {
        ///<inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(QueryResult<>))
            {
                return false;
            }

            return true;
        }

        ///<inheritdoc/>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type valueType = typeToConvert.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance
                (typeof(QueryResultConverter<>).MakeGenericType(valueType))!;

            return converter;
        }
    }
}
