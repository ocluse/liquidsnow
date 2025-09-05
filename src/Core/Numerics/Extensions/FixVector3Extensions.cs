#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;
public static partial class FixVector3Extensions
{
    #region FixedVector3 Operations

    /// <summary>
    /// Clamps each component of the vector to the range [-1, 1] in place and returns the modified vector.
    /// </summary>
    /// <param name="v">The vector to clamp.</param>
    /// <returns>The clamped vector with each component between -1 and 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ClampOneInPlace(this FixVector3 v)
    {
        v.X = v.X.ClampOne();
        v.Y = v.Y.ClampOne();
        v.Z = v.Z.ClampOne();
        return v;
    }

    public static FixVector3 ClampMagnitude(this FixVector3 value, Fix64 maxMagnitude)
    {
        return FixVector3.ClampMagnitude(value, maxMagnitude);
    }

    /// <summary>
    /// Checks if the distance between two vectors is less than or equal to a specified factor.
    /// </summary>
    /// <param name="me">The current vector.</param>
    /// <param name="other">The vector to compare distance to.</param>
    /// <param name="factor">The maximum allowable distance.</param>
    /// <returns>True if the distance between the vectors is less than or equal to the factor, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckDistance(this FixVector3 me, FixVector3 other, Fix64 factor)
    {
        var dis = FixVector3.Distance(me, other);
        return dis <= factor;
    }

    /// <inheritdoc cref="FixVector3.Rotate(FixVector3, FixVector3, FixQuaternion)" />
    public static FixVector3 Rotate(this FixVector3 source, FixVector3 position, FixQuaternion rotation)
    {
        return FixVector3.Rotate(source, position, rotation);
    }

    /// <inheritdoc cref="FixVector3.InverseRotate(FixVector3, FixVector3, FixQuaternion)" />
    public static FixVector3 InverseRotate(this FixVector3 source, FixVector3 position, FixQuaternion rotation)
    {
        return FixVector3.InverseRotate(source, position, rotation);
    }

    #endregion

    #region Conversion

    /// <inheritdoc cref="FixVector3.ToDegrees(FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ToDegrees(this FixVector3 radians)
    {
        return FixVector3.ToDegrees(radians);
    }

    /// <inheritdoc cref="FixVector3.ToRadians(FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ToRadians(this FixVector3 degrees)
    {
        return FixVector3.ToRadians(degrees);
    }

    /// <inheritdoc cref="FixVector3.Abs(FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Abs(this FixVector3 value)
    {
        return FixVector3.Abs(value);
    }

    /// <inheritdoc cref="FixVector3.Sign(FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Sign(FixVector3 value)
    {
        return FixVector3.Sign(value);
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
    public static bool FuzzyEqualAbsolute(this FixVector3 me, FixVector3 other, Fix64 allowedDifference)
    {
        return (me.X - other.X).Abs() <= allowedDifference &&
               (me.Y - other.Y).Abs() <= allowedDifference &&
               (me.Z - other.Z).Abs() <= allowedDifference;
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
    public static bool FuzzyEqual(this FixVector3 me, FixVector3 other, Fix64? percentage = null)
    {
        Fix64 p = percentage ?? Fix64.Epsilon;
        return me.X.FuzzyComponentEqual(other.X, p) &&
                me.Y.FuzzyComponentEqual(other.Y, p) &&
                me.Z.FuzzyComponentEqual(other.Z, p);
    }

    #endregion
}