#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;
public static class Fixed3x3Extensions
{
    #region Transformations

    /// <inheritdoc cref="Fix3x3.ExtractScale(Fix3x3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ExtractScale(this Fix3x3 matrix)
    {
        return Fix3x3.ExtractScale(matrix);
    }

    /// <inheritdoc cref="Fix3x3.SetScale(Fix3x3, FixVector3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 SetScale(this ref Fix3x3 matrix, FixVector3 localScale)
    {
        return matrix = Fix3x3.SetScale(matrix, localScale);
    }

    /// <inheritdoc cref="Fix3x3.SetGlobalScale(Fix3x3, FixVector3)" />
    public static Fix3x3 SetGlobalScale(this ref Fix3x3 matrix, FixVector3 globalScale)
    {
        return matrix = Fix3x3.SetGlobalScale(matrix, globalScale);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Compares two Fixed3x3 for approximate equality, allowing a fixed absolute difference between components.
    /// </summary>
    /// <param name="f1">The current Fixed3x3.</param>
    /// <param name="f2">The Fixed3x3 to compare against.</param>
    /// <param name="allowedDifference">The allowed absolute difference between each component.</param>
    /// <returns>True if the components are within the allowed difference, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FuzzyEqualAbsolute(this Fix3x3 f1, Fix3x3 f2, Fix64 allowedDifference)
    {
        return (f1.M00 - f2.M00).Abs() <= allowedDifference &&
               (f1.M01 - f2.M01).Abs() <= allowedDifference &&
               (f1.M02 - f2.M02).Abs() <= allowedDifference &&
               (f1.M10 - f2.M10).Abs() <= allowedDifference &&
               (f1.M11 - f2.M11).Abs() <= allowedDifference &&
               (f1.M12 - f2.M12).Abs() <= allowedDifference &&
               (f1.M20 - f2.M20).Abs() <= allowedDifference &&
               (f1.M21 - f2.M21).Abs() <= allowedDifference &&
               (f1.M22 - f2.M22).Abs() <= allowedDifference;
    }

    /// <summary>
    /// Compares two Fixed3x3 for approximate equality, allowing a fractional percentage (defaults to ~1%) difference between components.
    /// </summary>
    /// <param name="f1">The current Fixed3x3.</param>
    /// <param name="f2">The Fixed3x3 to compare against.</param>
    /// <param name="percentage">The allowed fractional difference (percentage) for each component.</param>
    /// <returns>True if the components are within the allowed percentage difference, false otherwise.</returns>
    public static bool FuzzyEqual(this Fix3x3 f1, Fix3x3 f2, Fix64? percentage = null)
    {
        Fix64 p = percentage ?? Fix64.Epsilon;
        return f1.M00.FuzzyComponentEqual(f2.M00, p) &&
               f1.M01.FuzzyComponentEqual(f2.M01, p) &&
               f1.M02.FuzzyComponentEqual(f2.M02, p) &&
               f1.M10.FuzzyComponentEqual(f2.M10, p) &&
               f1.M11.FuzzyComponentEqual(f2.M11, p) &&
               f1.M12.FuzzyComponentEqual(f2.M12, p) &&
               f1.M20.FuzzyComponentEqual(f2.M20, p) &&
               f1.M21.FuzzyComponentEqual(f2.M21, p) &&
               f1.M22.FuzzyComponentEqual(f2.M22, p);
    }

    #endregion
}
