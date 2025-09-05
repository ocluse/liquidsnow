#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;
public static partial class FixedQuaternionExtensions
{
    /// <inheritdoc cref="FixQuaternion.ToAngularVelocity" />
    public static FixVector3 ToAngularVelocity(
        this FixQuaternion currentRotation,
        FixQuaternion previousRotation,
        Fix64 deltaTime)
    {
        return FixQuaternion.ToAngularVelocity(currentRotation, previousRotation, deltaTime);
    }

    #region Equality

    /// <summary>
    /// Compares two quaternions for approximate equality, allowing a fixed absolute difference between components.
    /// </summary>
    /// <param name="q1">The current quaternion.</param>
    /// <param name="q2">The quaternion to compare against.</param>
    /// <param name="allowedDifference">The allowed absolute difference between each component.</param>
    /// <returns>True if the components are within the allowed difference, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FuzzyEqualAbsolute(this FixQuaternion q1, FixQuaternion q2, Fix64 allowedDifference)
    {
        return (q1.X - q2.X).Abs() <= allowedDifference &&
               (q1.Y - q2.Y).Abs() <= allowedDifference &&
               (q1.Z - q2.Z).Abs() <= allowedDifference &&
               (q1.W - q2.W).Abs() <= allowedDifference;
    }

    /// <summary>
    /// Compares two quaternions for approximate equality, allowing a fractional percentage (defaults to ~1%) difference between components.
    /// </summary>
    /// <param name="q1">The current quaternion.</param>
    /// <param name="q2">The quaternion to compare against.</param>
    /// <param name="percentage">The allowed fractional difference (percentage) for each component.</param>
    /// <returns>True if the components are within the allowed percentage difference, false otherwise.</returns>
    public static bool FuzzyEqual(this FixQuaternion q1, FixQuaternion q2, Fix64? percentage = null)
    {
        Fix64 p = percentage ?? Fix64.Epsilon;
        return q1.X.FuzzyComponentEqual(q2.X, p) &&
               q1.Y.FuzzyComponentEqual(q2.Y, p) &&
               q1.Z.FuzzyComponentEqual(q2.X, p) &&
               q1.W.FuzzyComponentEqual(q2.W, p);
    }

    #endregion
}
