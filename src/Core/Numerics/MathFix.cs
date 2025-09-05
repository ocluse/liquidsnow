#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics;


public partial class MathFix
{
    #region Fields and Constants


    public const int NUM_BITS = 64;

    public const int SHIFT_AMOUNT_I = 32;
    public const uint MAX_SHIFTED_AMOUNT_UI = (uint)((1L << SHIFT_AMOUNT_I) - 1);
    public const ulong MASK_UL = (ulong)(ulong.MaxValue << SHIFT_AMOUNT_I);

    public const long MAX_VALUE_L = long.MaxValue; // Max possible value for Fix64
    public const long MIN_VALUE_L = long.MinValue; // Min possible value for Fix64

    public const long ONE_L = 1L << SHIFT_AMOUNT_I;

    // Precomputed scale factors
    public const float SCALE_FACTOR_F = 1.0f / ONE_L;
    public const double SCALE_FACTOR_D = 1.0 / ONE_L;
    public const decimal SCALE_FACTOR_M = 1.0m / ONE_L;

    /// <summary>
    /// Represents the smallest possible value that can be represented by the Fix64 format.
    /// </summary>
    /// <remarks>
    /// Precision of this type is 2^-SHIFT_AMOUNT, 
    /// i.e. 1 / (2^SHIFT_AMOUNT) where SHIFT_AMOUNT defines the fractional bits.
    /// </remarks>
    public const long PRECISION_L = 1L;

    /// <summary>
    ///  The smallest value that a Fix64 can have different from zero.
    /// </summary>
    /// <remarks>
    /// With the following rules:
    ///      anyValue + Epsilon = anyValue
    ///      anyValue - Epsilon = anyValue
    ///      0 + Epsilon = Epsilon
    ///      0 - Epsilon = -Epsilon
    ///  A value Between any number and Epsilon will result in an arbitrary number due to truncating errors.
    /// </remarks>
    public const long EPSILON_L = 1L << (SHIFT_AMOUNT_I - 20); //~1E-06f

    #endregion

    #region FixedMath Operations

    /// <summary>
    /// Produces a value with the magnitude of the first argument and the sign of the second argument.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 CopySign(Fix64 x, Fix64 y)
    {
        return y >= Fix64.Zero ? x.Abs() : -x.Abs();
    }

    /// <summary>
    /// Clamps value between 0 and 1 and returns value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Clamp01(Fix64 value)
    {
        return value < Fix64.Zero ? Fix64.Zero : value > Fix64.One ? Fix64.One : value;
    }

    /// <summary>
    /// Clamps a fixed-point value between the given minimum and maximum values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Clamp(Fix64 f1, Fix64 min, Fix64 max)
    {
        return f1 < min ? min : f1 > max ? max : f1;
    }

    /// <summary>
    /// Clamps a value to the inclusive range [min, max].
    /// </summary>
    /// <typeparam name="T">The type of the value, must implement IComparable&lt;T&gt;.</typeparam>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(max) > 0) return max;
        if (value.CompareTo(min) < 0) return min;
        return value;
    }

    /// <summary>
    /// Clamps the value between -1 and 1 inclusive.
    /// </summary>
    /// <param name="f1">The Fix64 value to clamp.</param>
    /// <returns>Returns a value clamped between -1 and 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 ClampOne(Fix64 f1)
    {
        return f1 > Fix64.One ? Fix64.One : f1 < -Fix64.One ? -Fix64.One : f1;
    }

    /// <summary>
    /// Returns the absolute value of a Fix64 number.
    /// </summary>
    public static Fix64 Abs(Fix64 value)
    {
        // For the minimum value, return the max to avoid overflow
        if (value.RawValue == MIN_VALUE_L)
            return Fix64.MAX_VALUE;

        // Use branchless absolute value calculation
        long mask = value.RawValue >> 63; // If negative, mask will be all 1s; if positive, all 0s
        return Fix64.FromRaw((value.RawValue + mask) ^ mask);
    }

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to the specified number.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Ceiling(Fix64 value)
    {
        bool hasFractionalPart = (value.RawValue & MAX_SHIFTED_AMOUNT_UI) != 0;
        return hasFractionalPart ? value.Floor() + Fix64.One : value;
    }

    /// <summary>
    /// Returns the largest integer less than or equal to the specified number (floor function).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Floor(Fix64 value)
    {
        // Efficiently zeroes out the fractional part
        return Fix64.FromRaw((long)((ulong)value.RawValue & MathFix.MASK_UL));
    }

    /// <summary>
    /// Returns the larger of two fixed-point values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Max(Fix64 f1, Fix64 f2)
    {
        return f1 >= f2 ? f1 : f2;
    }

    /// <summary>
    /// Returns the smaller of two fixed-point values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Min(Fix64 a, Fix64 b)
    {
        return (a < b) ? a : b;
    }

    /// <summary>
    /// Rounds a fixed-point number to the nearest integral value, based on the specified rounding mode.
    /// </summary>
    public static Fix64 Round(Fix64 value, MidpointRounding mode = MidpointRounding.ToEven)
    {
        long fractionalPart = value.RawValue & MAX_SHIFTED_AMOUNT_UI;
        Fix64 integralPart = value.Floor();
        if (fractionalPart < Fix64.Half.RawValue)
            return integralPart;

        if (fractionalPart > Fix64.Half.RawValue)
            return integralPart + Fix64.One;

        // When value is exactly Fix64.Halfway between two numbers
        return mode switch
        {
            MidpointRounding.AwayFromZero => value.RawValue > 0 ? integralPart + Fix64.One : integralPart - Fix64.One,// If it's exactly Fix64.Halfway, round away from Fix64.Zero
            _ => (integralPart.RawValue & ONE_L) == 0 ? integralPart : integralPart + Fix64.One,// Rounds to the nearest even number (default behavior)
        };
    }

    /// <summary>
    /// Rounds a fixed-point number to a specific number of decimal places.
    /// </summary>
    public static Fix64 RoundToPrecision(Fix64 value, int decimalPlaces, MidpointRounding mode = MidpointRounding.ToEven)
    {
        if (decimalPlaces < 0 || decimalPlaces >= Pow10Lookup.Length)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places out of range.");

        int factor = Pow10Lookup[decimalPlaces];
        Fix64 scaled = value * factor;
        long rounded = Round(scaled, mode).RawValue;
        return new Fix64(rounded + (factor / 2)) / factor;
    }

    /// <summary>
    /// Squares the Fix64 value.
    /// </summary>
    /// <param name="value">The Fix64 value to square.</param>
    /// <returns>The squared value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Squared(Fix64 value)
    {
        return value * value;
    }

    /// <summary>
    /// Adds two fixed-point numbers without performing overflow checking.
    /// </summary>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 FastAdd(Fix64 x, Fix64 y)
    {
        return Fix64.FromRaw(x.RawValue + y.RawValue);
    }

    /// <summary>
    /// Subtracts two fixed-point numbers without performing overflow checking.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 FastSub(Fix64 x, Fix64 y)
    {
        return Fix64.FromRaw(x.RawValue - y.RawValue);
    }

    /// <summary>
    /// Multiplies two fixed-point numbers without overflow checking for performance-critical scenarios.
    /// </summary>
    public static Fix64 FastMul(Fix64 x, Fix64 y)
    {
        long xl = x.RawValue;
        long yl = y.RawValue;

        // Split values into high and low bits for long multiplication
        ulong xlo = (ulong)(xl & MAX_SHIFTED_AMOUNT_UI);
        long xhi = xl >> SHIFT_AMOUNT_I;
        ulong ylo = (ulong)(yl & MAX_SHIFTED_AMOUNT_UI);
        long yhi = yl >> SHIFT_AMOUNT_I;

        // Perform partial products
        ulong lolo = xlo * ylo;
        long lohi = (long)xlo * yhi;
        long hilo = xhi * (long)ylo;
        long hihi = xhi * yhi;

        // Combine the results
        ulong loResult = lolo >> SHIFT_AMOUNT_I;
        long midResult1 = lohi;
        long midResult2 = hilo;
        long hiResult = hihi << SHIFT_AMOUNT_I;

        long sum = (long)loResult + midResult1 + midResult2 + hiResult;
        return Fix64.FromRaw(sum);
    }

    /// <summary>
    /// Fast modulus without the checks performed by the '%' operator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 FastMod(Fix64 x, Fix64 y)
    {
        return Fix64.FromRaw(x.RawValue % y.RawValue);
    }

    /// <summary>
    /// Performs a smooth step interpolation between two values.
    /// </summary>
    /// <remarks>
    /// The interpolation follows a cubic Hermite curve where the function starts at `a`,
    /// accelerates, and then decelerates towards `b`, ensuring smooth transitions.
    /// </remarks>
    /// <param name="a">The starting value.</param>
    /// <param name="b">The ending value.</param>
    /// <param name="t">A value between 0 and 1 that represents the interpolation factor.</param>
    /// <returns>The interpolated value between `a` and `b`.</returns>
    public static Fix64 SmoothStep(Fix64 a, Fix64 b, Fix64 t)
    {
        t = t * t * (Fix64.Three - Fix64.Two * t);
        return LinearInterpolate(a, b, t);
    }

    /// <summary>
    /// Performs a cubic Hermite interpolation between two points, using specified tangents.
    /// </summary>
    /// <remarks>
    /// This method interpolates smoothly between `p0` and `p1` while considering the tangents `m0` and `m1`.
    /// It is useful for animation curves and smooth motion transitions.
    /// </remarks>
    /// <param name="p0">The first point.</param>
    /// <param name="p1">The second point.</param>
    /// <param name="m0">The tangent (slope) at `p0`.</param>
    /// <param name="m1">The tangent (slope) at `p1`.</param>
    /// <param name="t">A value between 0 and 1 that represents the interpolation factor.</param>
    /// <returns>The interpolated value between `p0` and `p1`.</returns>
    public static Fix64 CubicInterpolate(Fix64 p0, Fix64 p1, Fix64 m0, Fix64 m1, Fix64 t)
    {
        Fix64 t2 = t * t;
        Fix64 t3 = t2 * t;
        return (Fix64.Two * p0 - Fix64.Two * p1 + m0 + m1) * t3
             + (-Fix64.Three * p0 + Fix64.Three * p1 - Fix64.Two * m0 - m1) * t2
             + m0 * t + p0;
    }

    /// <summary>
    /// Performs linear interpolation between two fixed-point values based on the interpolant t (0 greater or equal to `t` and less than or equal to 1).
    /// </summary>
    public static Fix64 LinearInterpolate(Fix64 from, Fix64 to, Fix64 t)
    {
        if (t.RawValue >= ONE_L)
            return to;
        if (t.RawValue <= 0)
            return from;

        return (to * t) + (from * (Fix64.One - t));
    }

    /// <summary>
    /// Moves a value from 'from' to 'to' by a maximum step of 'maxAmount'. 
    /// Ensures the value does not exceed 'to'.
    /// </summary>
    public static Fix64 MoveTowards(Fix64 from, Fix64 to, Fix64 maxAmount)
    {
        if (from < to)
        {
            from += maxAmount;
            if (from > to)
                from = to;
        }
        else if (from > to)
        {
            from -= maxAmount;
            if (from < to)
                from = to;
        }

        return Fix64.FromRaw(from.RawValue);
    }

    /// <summary>
    /// Adds two <see cref="long"/> values and checks for overflow.
    /// If an overflow occurs during addition, the <paramref name="overflow"/> parameter is set to true.
    /// </summary>
    /// <param name="x">The first operand to add.</param>
    /// <param name="y">The second operand to add.</param>
    /// <param name="overflow">
    /// A reference parameter that is set to true if an overflow is detected during the addition.
    /// The existing value of <paramref name="overflow"/> is preserved if already true.
    /// </param>
    /// <returns>The sum of <paramref name="x"/> and <paramref name="y"/>.</returns>
    /// <remarks>
    /// Overflow is detected by checking for a change in the sign bit that indicates a wrap-around.
    /// Additionally, a special check is performed for adding <see cref="Fix64.MIN_VALUE"/> and -1, 
    /// as this is a known edge case for overflow.
    /// </remarks>
    public static long AddOverflowHelper(long x, long y, ref bool overflow)
    {
        long sum = x + y;
        // Check for overflow using sign bit changes
        overflow |= ((x ^ y ^ sum) & MIN_VALUE_L) != 0;
        // Special check for the case when x is long.Fix64.MinValue and y is negative
        if (x == long.MinValue && y == -1)
            overflow = true;
        return sum;
    }

    #endregion
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member