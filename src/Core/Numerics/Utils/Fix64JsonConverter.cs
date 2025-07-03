#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Utils;
public class Fix64JsonConverter : JsonConverter<Fix64>
{
    public override Fix64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        long rawValue = reader.GetInt64();
        return Fix64.FromRaw(rawValue);
    }

    public override void Write(Utf8JsonWriter writer, Fix64 value, JsonSerializerOptions options)
    {
        long rawValue = value.RawValue;
        writer.WriteNumberValue(rawValue);
    }
}
