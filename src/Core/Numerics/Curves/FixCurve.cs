#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Curves;

/// <summary>
/// A deterministic fixed-point curve that interpolates values between keyframes.
/// Used for animations, physics calculations, and procedural data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FixCurve"/> with a specified interpolation mode.
/// </remarks>
/// <param name="mode">The interpolation method to use.</param>
/// <param name="keyframes">The keyframes defining the curve.</param>
[Serializable]
[method: JsonConstructor]
public class FixCurve(FixCurveMode mode, params FixCurveKey[] keyframes) : IEquatable<FixCurve>
{
    public FixCurveMode Mode { get; private set; } = mode;

    public FixCurveKey[] Keyframes { get; private set; } = [.. keyframes.OrderBy(k => k.Time)];

    /// <summary>
    /// Initializes a new instance of the <see cref="FixCurve"/> with a default linear interpolation mode.
    /// </summary>
    /// <param name="keyframes">The keyframes defining the curve.</param>
    public FixCurve(params FixCurveKey[] keyframes)
        : this(FixCurveMode.Linear, keyframes) { }

    /// <summary>
    /// Evaluates the curve at a given time using the specified interpolation mode.
    /// </summary>
    /// <param name="time">The time at which to evaluate the curve.</param>
    /// <returns>The interpolated value at the given time.</returns>
    public Fix64 Evaluate(Fix64 time)
    {
        if (Keyframes.Length == 0) return Fix64.One;

        // Clamp input within the keyframe range
        if (time <= Keyframes[0].Time) return Keyframes[0].Value;
        if (time >= Keyframes[^1].Time) return Keyframes[^1].Value;

        // Find the surrounding keyframes
        for (int i = 0; i < Keyframes.Length - 1; i++)
        {
            if (time >= Keyframes[i].Time && time < Keyframes[i + 1].Time)
            {
                // Compute interpolation factor
                Fix64 t = (time - Keyframes[i].Time) / (Keyframes[i + 1].Time - Keyframes[i].Time);

                // Choose interpolation method
                return Mode switch
                {
                    FixCurveMode.Step => Keyframes[i].Value,// Immediate transition
                    FixCurveMode.Smooth => MathFix.SmoothStep(Keyframes[i].Value, Keyframes[i + 1].Value, t),
                    FixCurveMode.Cubic => MathFix.CubicInterpolate(
                                                    Keyframes[i].Value, Keyframes[i + 1].Value,
                                                    Keyframes[i].OutTangent, Keyframes[i + 1].InTangent, t),
                    _ => MathFix.LinearInterpolate(Keyframes[i].Value, Keyframes[i + 1].Value, t),
                };
            }
        }

        return Fix64.One; // Fallback (should never be hit)
    }

    public bool Equals(FixCurve? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Mode == other.Mode && Keyframes.SequenceEqual(other.Keyframes);
    }

    public override bool Equals(object? obj) => obj is FixCurve other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)Mode;
            foreach (var key in Keyframes)
                hash = hash * 31 ^ key.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(FixCurve left, FixCurve right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(FixCurve left, FixCurve right) => !(left == right);
}