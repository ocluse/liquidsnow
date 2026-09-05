using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static partial class MathFix
{
    #region Fields and Constants


    public static readonly int[] Pow10Lookup = [
            1,           // 10^0 = 1
            10,          // 10^1 = 10
            100,         // 10^2 = 100
            1000,        // 10^3 = 1000
            10000,       // 10^4 = 10000
            100000,      // 10^5 = 100000
            1000000,     // 10^6 = 1000000
            10000000,    // 10^7
            100000000,   // 10^8
            1000000000,  // 10^9
        ];

    // Trigonometric and logarithmic constants
    internal const double PI_D = 3.14159265358979323846;
    public static readonly Fix64 PI = (Fix64)PI_D;
    public static readonly Fix64 TwoPI = PI * 2;
    public static readonly Fix64 PiOver2 = PI / 2;
    public static readonly Fix64 PiOver3 = PI / 3;
    public static readonly Fix64 PiOver4 = PI / 4;
    public static readonly Fix64 PiOver6 = PI / 6;
    public static readonly Fix64 Ln2 = (Fix64)0.6931471805599453;  // Natural logarithm of 2

    public static readonly Fix64 LOG_2_MAX = new(31L * ONE_L);
    public static readonly Fix64 LOG_2_MIN = new(-33L * ONE_L);

    internal const double DEG2RAD_D = 0.01745329251994329576;  // π / 180
    public static readonly Fix64 Deg2Rad = (Fix64)DEG2RAD_D;  // Degrees to radians conversion factor
    internal const double RAD2DEG_D = 57.2957795130823208767;  // 180 / π
    public static readonly Fix64 Rad2Deg = (Fix64)RAD2DEG_D;  // Radians to degrees conversion factor

    #endregion

    #region FixedTrigonometry Operations

    /// <summary>
    /// Raises the base number b to the power of exp.
    /// Uses logarithms to compute power efficiently for fixed-point values.
    /// </summary>
    /// <exception cref="DivideByZeroException">
    /// The base was zero with a negative exponent.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The base was negative with a nonzero exponent.
    /// </exception>
    public static Fix64 Pow(Fix64 b, Fix64 exp)
    {
        if (b == Fix64.One)
            return Fix64.One;

        if (exp.RawValue == 0)
            return Fix64.One;

        if (b.RawValue == 0)
        {
            if (exp.RawValue < 0)
                throw new DivideByZeroException("Cannot raise 0 to a negative power.");

            return Fix64.Zero;
        }

        Fix64 log2 = Log2(b);  // Calculate logarithm base 2
        return Pow2(exp * log2);  // Raise 2 to the power of log2 result
    }

    /// <summary>
    /// Raises 2 to the power of x.
    /// Provides high accuracy for small values of x.
    /// </summary>
    public static Fix64 Pow2(Fix64 x)
    {
        if (x >= LOG_2_MAX) return Fix64.MAX_VALUE;
        if (x <= LOG_2_MIN) return Fix64.Zero;
        int integral = (int)Floor(x);
        decimal fraction = (decimal)x - integral;
        decimal term = 1m, result = 1m;
        decimal exponent = fraction * 0.6931471805599453094172321215m;
        for (int i = 1; i <= 28; i++)
        {
            term = term * exponent / i;
            result += term;
        }
        if (integral >= 0) result *= 1L << integral;
        else result /= 1L << -integral;
        return Fix64.FromDecimal(result, saturating: true);
    }

    /// <summary>
    /// Returns the base-2 logarithm of a specified number.
    /// Provides at least 9 decimals of accuracy.
    /// </summary>
    /// <remarks>
    /// This implementation is based on Clay. S. Turner's fast binary logarithm algorithm 
    /// (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal Processing Mag., pp. 124,140, Sep. 2010.)
    /// </remarks>
    public static Fix64 Log2(Fix64 x)
    {
        if (x.RawValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Cannot compute logarithm of non-positive number.");

        long b = 1U << (SHIFT_AMOUNT_I - 1);  // Initial value for binary logarithm
        long y = 0;  // Result accumulator
        long rawX = x.RawValue;

        // Adjust rawX to the correct range [1, 2)
        while (rawX < ONE_L)
        {
            rawX <<= 1;
            y -= ONE_L;
        }

        while (rawX >= (ONE_L << 1))
        {
            rawX >>= 1;
            y += ONE_L;
        }

        Fix64 z = Fix64.FromRaw(rawX);  // Remaining fraction

        for (int i = 0; i < SHIFT_AMOUNT_I; i++)
        {
            z = FastMul(z, z);
            if (z.RawValue >= (ONE_L << 1))
            {
                z = Fix64.FromRaw(z.RawValue >> 1);
                y += b;
            }
            b >>= 1;
        }

        return Fix64.FromRaw(y);
    }

    /// <summary>
    /// Returns the natural logarithm of a specified fixed-point number.
    /// Provides at least 7 decimals of accuracy.
    /// </summary>
    public static Fix64 Ln(Fix64 x)
    {
        if (x.RawValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(x),"Cannot compute logarithm of non-positive number.");

        return Log2(x) * Ln2;
    }

    /// <summary>
    /// Returns the square root of a specified fixed-point number.
    /// </summary>
    public static Fix64 Sqrt(Fix64 x)
    {
        if (x < Fix64.Zero) throw new ArgumentOutOfRangeException(nameof(x));
        return Fix64.FromRaw((long)SqrtRounded((UInt128)(ulong)x.RawValue << SHIFT_AMOUNT_I));
    }

    /// <summary>
    /// Converts a value in radians to degrees.
    /// </summary>
    /// <remarks>
    /// Uses decimal guard digits and saturates if the result is outside the fixed-point range.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 RadToDeg(Fix64 rad)
    {
        return Fix64.FromDecimal((decimal)rad * (180m / DecimalPI), saturating: true);
    }

    /// <summary>
    /// Converts a value in degrees to radians.
    /// </summary>
    /// <remarks>
    /// Uses decimal guard digits before fixed-point quantization.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 DegToRad(Fix64 deg)
    {
        return Fix64.FromDecimal((decimal)deg * (DecimalPI / 180m));
    }

    /// <summary>
    /// Computes the sine of a given angle in radians using a reduced-angle Taylor series.
    /// </summary>
    /// <param name="x">The angle in radians.</param>
    /// <returns>The sine of the given angle, in fixed-point format.</returns>
    /// <remarks>Reduces the angle before evaluating a series with decimal guard digits.</remarks>
    public static Fix64 Sin(Fix64 x)
    {
        if (x == Fix64.Zero || x == PI || x == -PI || x == TwoPI || x == -TwoPI) return Fix64.Zero;
        if (x == PiOver2) return Fix64.One;
        if (x == -PiOver2) return -Fix64.One;
        return SinDecimal((decimal)x);
    }

    private static Fix64 SinDecimal(decimal angle)
    {
        // Reduce with guard digits: a Q32.32 period accumulates phase error for large angles.
        angle %= 2 * DecimalPI;
        if (angle > DecimalPI) angle -= 2 * DecimalPI;
        if (angle < -DecimalPI) angle += 2 * DecimalPI;
        if (angle > DecimalPI / 2) angle = DecimalPI - angle;
        if (angle < -DecimalPI / 2) angle = -DecimalPI - angle;
        // Decimal intermediates provide deterministic guard digits before Q32.32 quantization.
        decimal square = angle * angle;
        decimal term = angle, result = angle;
        for (int i = 1; i <= 12; i++)
        {
            term = -term * square / ((2 * i) * (2 * i + 1));
            result += term;
        }
        return ClampOne(Fix64.FromDecimal(result));
    }

    /// <summary>
    /// Computes the cosine of a given angle in radians using a sine-based identity transformation.
    /// </summary>
    /// <param name="x">The angle in radians.</param>
    /// <returns>The cosine of the given angle, in fixed-point format.</returns>
    /// <remarks>Uses a reduced-angle sine identity.</remarks>
    public static Fix64 Cos(Fix64 x)
    {
        return SinDecimal((decimal)x + DecimalPI / 2);
    }

    public static Fix64 SinToCos(Fix64 sin)
    {
        if (sin < -Fix64.One || sin > Fix64.One) throw new ArgumentOutOfRangeException(nameof(sin));
        return UnitCircleComplement(sin);
    }

    private static Fix64 UnitCircleComplement(Fix64 x)
    {
        // Preserve all bits of 1-x^2, particularly near either endpoint.
        Int128 square = (Int128)ONE_L * ONE_L - (Int128)x.RawValue * x.RawValue;
        return Fix64.FromRaw((long)SqrtRounded((UInt128)square));
    }

    /// <summary>
    /// Returns the tangent of x.
    /// </summary>
    /// <remarks>
    /// Throws DivideByZeroException when the quantized cosine is zero.
    /// </remarks>
    public static Fix64 Tan(Fix64 x)
    {
        return Sin(x) / Cos(x);
    }

    /// <summary>
    /// Returns the arc-sine of a fixed-point number x, which is the angle in radians 
    /// whose sine is x, using atan2 and sqrt.
    /// </summary>
    /// <param name="x">The input value (sine) whose arcsine is to be computed. Should be in the range [-1, 1].</param>
    /// <returns>The arc-sine of x in radians.</returns>
    /// <exception cref="ArithmeticException">Thrown if x is outside the domain [-1, 1].</exception>
    public static Fix64 Asin(Fix64 x)
    {
        if (x < -Fix64.One || x > Fix64.One) throw new ArithmeticException("Input out of domain for Asin.");
        if (x == Fix64.One) return PiOver2;
        if (x == -Fix64.One) return -PiOver2;
        return Atan2(x, UnitCircleComplement(x));
    }

    /// <summary>
    /// Returns the arccosine of the specified number x, calculated using a combination of the atan and sqrt functions.
    /// </summary>
    /// <param name="x">The input value whose arccosine is to be computed. Should be in the range [-1, 1].</param>
    /// <returns>The arccosine of x in radians.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if x is outside the domain [-1, 1].</exception>
    public static Fix64 Acos(Fix64 x)
    {
        if (x < -Fix64.One || x > Fix64.One) throw new ArgumentOutOfRangeException(nameof(x));
        if (x == Fix64.One) return Fix64.Zero;
        if (x == -Fix64.One) return PI;
        return Atan2(UnitCircleComplement(x), x);
    }

    /// <summary>
    /// Returns the arctangent of the specified number, using a more accurate approximation for larger values.
    /// This function has at least 7 decimals of accuracy.
    /// </summary>
    public static Fix64 Atan(Fix64 z)
    {
        return Fix64.FromDecimal(AtanDecimal((decimal)z));
    }

    /// <summary>
    /// Computes the angle whose tangent is the quotient of two specified numbers.
    /// </summary>
    /// <remarks>
    /// Uses decimal guard digits so a large y/x ratio cannot overflow before angle evaluation.
    /// </remarks>
    /// <param name="y">The y-coordinate of the point to which the angle is measured.</param>
    /// <param name="x">The x-coordinate of the point to which the angle is measured.</param>
    /// <returns>An angle, θ, measured in radians, such that -π ≤ θ ≤ π, and tan(θ) = y / x, 
    /// taking into account the quadrants of the inputs to determine the sign of the result.</returns>
    public static Fix64 Atan2(Fix64 y, Fix64 x)
    {
        if (x == Fix64.Zero)
            return y > Fix64.Zero ? PiOver2 : y < Fix64.Zero ? -PiOver2 : Fix64.Zero;
        decimal angle = AtanDecimal((decimal)y / (decimal)x);
        if (x < Fix64.Zero) angle += y >= Fix64.Zero ? DecimalPI : -DecimalPI;
        return Fix64.FromDecimal(angle);
    }

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
