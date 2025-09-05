#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;
public static partial class FixVector2Extensions
{
    #region FixedVector2 Operations

    /// <summary>
    /// Clamps each component of the vector to the range [-1, 1] in place and returns the modified vector.
    /// </summary>
    /// <param name="v">The vector to clamp.</param>
    /// <returns>The clamped vector with each component between -1 and 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 ClampOneInPlace(this FixVector2 v)
    {
        v.X = v.X.ClampOne();
        v.Y = v.Y.ClampOne();
        return v;
    }

    /// <summary>
    /// Checks if the distance between two vectors is less than or equal to a specified factor.
    /// </summary>
    /// <param name="me">The current vector.</param>
    /// <param name="other">The vector to compare distance to.</param>
    /// <param name="factor">The maximum allowable distance.</param>
    /// <returns>True if the distance between the vectors is less than or equal to the factor, false otherwise.</returns>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckDistance(this FixVector2 me, FixVector2 other, Fix64 factor)
    {
        var dis = FixVector2.Distance(me, other);
        return dis <= factor;
    }

    /// <inheritdoc cref="FixVector2.Rotate(FixVector2, Fix64)" />
    public static FixVector2 Rotate(this FixVector2 vec, Fix64 angleInRadians)
    {
        return FixVector2.Rotate(vec, angleInRadians);
    }

    /// <inheritdoc cref="FixVector2.Abs(FixVector2)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 Abs(this FixVector2 value)
    {
        return FixVector2.Abs(value);
    }

    /// <inheritdoc cref="FixVector2.Sign(FixVector2)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 Sign(this FixVector2 value)
    {
        return FixVector2.Sign(value);
    }

    #endregion

    #region Conversion

    /// <inheritdoc cref="FixVector2.ToDegrees(FixVector2)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 ToDegrees(this FixVector2 radians)
    {
        return FixVector2.ToDegrees(radians);
    }

    /// <inheritdoc cref="FixVector2.ToRadians(FixVector2)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 ToRadians(this FixVector2 degrees)
    {
        return FixVector2.ToRadians(degrees);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Compares two vectors for approximate equality, allowing a fixed absolute difference.
    /// </summary>
    /// <param name="me">The current vector.</param>
    /// <param name="other">The vector to compare against.</param>
    /// <param name="allowedDifference">The allowed absolute difference between each component.</param>
    /// <returns>True if the components are within the allowed difference, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FuzzyEqualAbsolute(this FixVector2 me, FixVector2 other, Fix64 allowedDifference)
    {
        return (me.X - other.X).Abs() <= allowedDifference &&
               (me.Y - other.Y).Abs() <= allowedDifference;
    }

    /// <summary>
    /// Compares two vectors for approximate equality, allowing a fractional difference (percentage).
    /// Handles zero components by only using the allowed percentage difference.
    /// </summary>
    /// <param name="me">The current vector.</param>
    /// <param name="other">The vector to compare against.</param>
    /// <param name="percentage">The allowed fractional difference (percentage) for each component.</param>
    /// <returns>True if the components are within the allowed percentage difference, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FuzzyEqual(this FixVector2 me, FixVector2 other, Fix64? percentage = null)
    {
        Fix64 p = percentage ?? Fix64.Epsilon;
        return me.X.FuzzyComponentEqual(other.X, p) && me.Y.FuzzyComponentEqual(other.Y, p);
    }

    #endregion
}