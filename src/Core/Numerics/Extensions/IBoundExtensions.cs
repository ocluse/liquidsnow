#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member


using Ocluse.LiquidSnow.Numerics.Bounds;
using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;

public static class IBoundExtensions
{
    /// <inheritdoc cref="IFixBound.ProjectPoint(FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ProjectPointWithinBounds(this IFixBound bounds, FixVector3 point)
    {
        return new FixVector3(
            MathFix.Clamp(point.X, bounds.Min.X, bounds.Max.X),
            MathFix.Clamp(point.Y, bounds.Min.Y, bounds.Max.Y),
            MathFix.Clamp(point.Z, bounds.Min.Z, bounds.Max.Z)
        );
    }
}