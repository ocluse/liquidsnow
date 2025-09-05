using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;

/// <summary>
/// Extensions for the Fix64 type.
/// </summary>
public static class Fix64Extensions
{
    #region Fix64 Operations

    /// <inheritdoc cref="Fix64.Sign(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this Fix64 value)
    {
        return Fix64.Sign(value);
    }

    /// <inheritdoc cref="Fix64.IsInteger(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInteger(this Fix64 value)
    {
        return Fix64.IsInteger(value);
    }

    /// <inheritdoc cref="MathFix.Squared(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Squared(this Fix64 value)
    {
        return MathFix.Squared(value);
    }

    /// <inheritdoc cref="MathFix.Round(Fix64, MidpointRounding)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Round(this Fix64 value, MidpointRounding mode = MidpointRounding.ToEven)
    {
        return MathFix.Round(value, mode);
    }

    /// <inheritdoc cref="MathFix.RoundToPrecision(Fix64, int, MidpointRounding)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 RoundToPrecision(this Fix64 value, int places, MidpointRounding mode = MidpointRounding.ToEven)
    {
        return MathFix.RoundToPrecision(value, places, mode);
    }

    /// <inheritdoc cref="MathFix.ClampOne(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 ClampOne(this Fix64 f1)
    {
        return MathFix.ClampOne(f1);
    }

    /// <inheritdoc cref="MathFix.Clamp01(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Clamp01(this Fix64 f1)
    {
        return MathFix.Clamp01(f1);
    }

    /// <inheritdoc cref="MathFix.Abs(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Abs(this Fix64 value)
    {
        return MathFix.Abs(value);
    }

    /// <summary>
    /// Checks if the absolute value of x is less than y.
    /// </summary>
    /// <param name="x">The value to compare.</param>
    /// <param name="y">The comparison threshold.</param>
    /// <returns>True if |x| &lt; y; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AbsLessThan(this Fix64 x, Fix64 y)
    {
        return x.Abs() < y;
    }

    /// <inheritdoc cref="MathFix.FastAdd(Fix64, Fix64)" />
    public static Fix64 FastAdd(this Fix64 a, Fix64 b)
    {
        return MathFix.FastAdd(a, b);
    }

    /// <inheritdoc cref="MathFix.FastSub(Fix64, Fix64)" />
    public static Fix64 FastSub(this Fix64 a, Fix64 b)
    {
        return MathFix.FastSub(a, b);
    }

    /// <inheritdoc cref="MathFix.FastMul(Fix64, Fix64)" />
    public static Fix64 FastMul(this Fix64 a, Fix64 b)
    {
        return MathFix.FastMul(a, b);
    }

    /// <inheritdoc cref="MathFix.FastMod(Fix64, Fix64)" />
    public static Fix64 FastMod(this Fix64 a, Fix64 b)
    {
        return MathFix.FastMod(a, b);
    }

    /// <inheritdoc cref="MathFix.Floor(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Floor(this Fix64 value)
    {
        return MathFix.Floor(value);
    }

    /// <inheritdoc cref="MathFix.Ceiling(Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Ceiling(this Fix64 value)
    {
        return MathFix.Ceiling(value);
    }

    /// <summary>
    /// Rounds the Fix64 value to the nearest integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this Fix64 x)
    {
        return (int)MathFix.Round(x);
    }

    /// <summary>
    /// Rounds up the Fix64 value to the nearest integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilToInt(this Fix64 x)
    {
        return (int)MathFix.Ceiling(x);
    }

    /// <summary>
    /// Rounds down the Fix64 value to the nearest integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(this Fix64 x)
    {
        return (int)x.Floor();
    }

    #endregion

    #region Conversion

    /// <summary>
    /// Converts the Fix64 value to a string formatted to 2 decimal places.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToFormattedString(this Fix64 f1)
    {
        return f1.ToPreciseFloat().ToString("0.##");
    }

    /// <summary>
    /// Converts the Fix64 value to a double with specified decimal precision.
    /// </summary>
    /// <param name="f1">The Fix64 value to convert.</param>
    /// <param name="precision">The number of decimal places to round to.</param>
    /// <returns>The formatted double value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToFormattedDouble(this Fix64 f1, int precision = 2)
    {
        return Math.Round((double)f1, precision, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Converts the Fix64 value to a float with 2 decimal points of precision.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFormattedFloat(this Fix64 f1)
    {
        return (float)f1.ToFormattedDouble();
    }

    /// <summary>
    /// Converts the Fix64 value to a precise float representation (without rounding).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPreciseFloat(this Fix64 f1)
    {
        return (float)(double)f1;
    }

    /// <summary>
    /// Converts the angle in degrees to radians.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 ToRadians(this Fix64 angleInDegrees)
    {
        return MathFix.DegToRad(angleInDegrees);
    }

    /// <summary>
    /// Converts the angle in radians to degree.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 ToDegree(this Fix64 angleInRadians)
    {
        return MathFix.RadToDeg(angleInRadians);
    }

    #endregion

    #region Equality

    /// <summary>
    /// Checks if the value is greater than epsilon (positive or negative).
    /// Useful for determining if a value is effectively non-zero with a given precision.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MoreThanEpsilon(this Fix64 d)
    {
        return d.Abs() > Fix64.Epsilon;
    }

    /// <summary>
    /// Checks if the value is less than epsilon (i.e., effectively zero).
    /// Useful for determining if a value is close enough to zero with a given precision.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LessThanEpsilon(this Fix64 d)
    {
        return d.Abs() < Fix64.Epsilon;
    }

    /// <summary>
    /// Helper method to compare individual vector components for approximate equality, allowing a fractional difference.
    /// Handles zero components by only using the allowed percentage difference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FuzzyComponentEqual(this Fix64 a, Fix64 b, Fix64 percentage)
    {
        var diff = (a - b).Abs();
        var allowedErr = a.Abs() * percentage;
        // Compare directly to percentage if a is zero
        // Otherwise, use percentage of a's magnitude
        return a == Fix64.Zero ? diff <= percentage : diff <= allowedErr;
    }

    #endregion
}
