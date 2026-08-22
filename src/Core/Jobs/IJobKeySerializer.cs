using System.Globalization;

namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Converts job and queue identifiers into stable, indexable strings.
/// </summary>
public interface IJobKeySerializer
{
    /// <summary>
    /// Serializes an identifier.
    /// </summary>
    string Serialize(object value);
}

/// <summary>
/// Default key serializer for common scalar identifier types.
/// </summary>
public sealed class DefaultJobKeySerializer : IJobKeySerializer
{
    /// <inheritdoc />
    public string Serialize(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value switch
        {
            string text => $"string:{text}",
            Guid guid => $"guid:{guid:D}",
            byte number => $"byte:{number.ToString(CultureInfo.InvariantCulture)}",
            sbyte number => $"sbyte:{number.ToString(CultureInfo.InvariantCulture)}",
            short number => $"int16:{number.ToString(CultureInfo.InvariantCulture)}",
            ushort number => $"uint16:{number.ToString(CultureInfo.InvariantCulture)}",
            int number => $"int32:{number.ToString(CultureInfo.InvariantCulture)}",
            uint number => $"uint32:{number.ToString(CultureInfo.InvariantCulture)}",
            long number => $"int64:{number.ToString(CultureInfo.InvariantCulture)}",
            ulong number => $"uint64:{number.ToString(CultureInfo.InvariantCulture)}",
            Enum enumValue => $"enum:{enumValue.GetType().FullName}:{Enum.Format(enumValue.GetType(), enumValue, "D")}",
            _ => throw new NotSupportedException(
                $"Job identifier type '{value.GetType().FullName}' is not supported by the default key serializer. " +
                "Register a custom IJobKeySerializer for compound identifiers.")
        };
    }
}
