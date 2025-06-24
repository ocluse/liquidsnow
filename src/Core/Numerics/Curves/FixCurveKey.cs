#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Curves;
/// <summary>
/// Represents a keyframe in a <see cref="FixCurve"/>, defining a value at a specific time.
/// </summary>
/// <remarks>
/// Creates a keyframe with optional tangents for cubic interpolation.
/// </remarks>
[Serializable]
[method: JsonConstructor]
public struct FixCurveKey(Fix64 time, Fix64 value, Fix64 inTangent, Fix64 outTangent) : IEquatable<FixCurveKey>
{
    /// <summary>The time at which this keyframe occurs.</summary>
    [JsonInclude]
    public Fix64 Time = time;

    /// <summary>The value of the curve at this keyframe.</summary>
    [JsonInclude]
    public Fix64 Value = value;

    /// <summary>The incoming tangent for cubic interpolation.</summary>
    [JsonInclude]
    public Fix64 InTangent = inTangent;

    /// <summary>The outgoing tangent for cubic interpolation.</summary>
    [JsonInclude]
    public Fix64 OutTangent = outTangent;

    /// <summary>
    /// Creates a keyframe with a specified time and value.
    /// </summary>
    public FixCurveKey(double time, double value)
        : this(new Fix64(time), new Fix64(value)) { }

    /// <summary>
    /// Creates a keyframe with optional tangents for cubic interpolation.
    /// </summary>
    public FixCurveKey(double time, double value, double inTangent, double outTangent)
        : this(new Fix64(time), new Fix64(value), new Fix64(inTangent), new Fix64(outTangent)) { }

    /// <summary>
    /// Creates a keyframe with a specified time and value.
    /// </summary>
    public FixCurveKey(Fix64 time, Fix64 value)
        : this(time, value, Fix64.Zero, Fix64.Zero) { }

    public readonly bool Equals(FixCurveKey other)
    {
        return Time == other.Time &&
               Value == other.Value &&
               InTangent == other.InTangent &&
               OutTangent == other.OutTangent;
    }

    public override readonly bool Equals(object? obj) => obj is FixCurveKey other && Equals(other);

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Time, Value, InTangent, OutTangent);
    }

    public static bool operator ==(FixCurveKey left, FixCurveKey right) => left.Equals(right);

    public static bool operator !=(FixCurveKey left, FixCurveKey right) => !(left == right);
}
