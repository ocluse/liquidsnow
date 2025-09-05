#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Utils;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;

/// <summary>
/// A fixed-point number structure that represents a 64-bit signed integer with a fixed number of fractional bits.
/// </summary>
[Serializable]
[JsonConverter(typeof(Fix64JsonConverter))]
public partial struct Fix64 : IEquatable<Fix64>, IComparable<Fix64>, IEqualityComparer<Fix64>
{
    #region Fields and Constants

    private long _rawValue;

    public static readonly Fix64 MAX_VALUE = new(MathFix.MAX_VALUE_L);

    public static readonly Fix64 MIN_VALUE = new(MathFix.MIN_VALUE_L);

    public static readonly Fix64 One = new(MathFix.ONE_L);

    public static readonly Fix64 Two = One * 2;
    public static readonly Fix64 Three = One * 3;
    public static readonly Fix64 Half = One / 2;
    public static readonly Fix64 Quarter = One / 4;
    public static readonly Fix64 Eighth = One / 8;
    public static readonly Fix64 Zero = new(0);

    /// <inheritdoc cref="MathFix.EPSILON_L" />
    public static readonly Fix64 Epsilon = new(MathFix.EPSILON_L);
    /// <inheritdoc cref="MathFix.PRECISION_L" />
    public static readonly Fix64 Precision = new(MathFix.PRECISION_L);

    #endregion

    #region Constructors

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Fix64(long rawValue)
    {
        _rawValue = rawValue;
    }

    /// <summary>
    /// Constructs a Fix64 from an integer, with the fractional part set to zero.
    /// </summary>
    /// <param name="value">Integer value to convert to </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fix64(int value) : this((long)value << MathFix.SHIFT_AMOUNT_I) { }

    /// <summary>
    /// Constructs a Fix64 from a double-precision floating-point value.
    /// </summary>
    /// <param name="value">Double value to convert to </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fix64(double value) : this((long)Math.Round((double)value * MathFix.ONE_L)) { }

    #endregion

    #region Properties (Instance)

    /// <summary>
    /// The underlying raw long value representing the fixed-point number.
    /// </summary>
    public readonly long RawValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _rawValue;
    }

    #endregion

    #region Methods (Instance)

    /// <summary>
    /// Offsets the current Fix64 by an integer value.
    /// </summary>
    /// <param name="x">The integer value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Offset(int x)
    {
        _rawValue += (long)x << MathFix.SHIFT_AMOUNT_I;
    }

    /// <summary>
    /// Returns the raw value as a string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string RawToString()
    {
        return _rawValue.ToString();
    }

    #endregion

    #region Fix64 Operations

    /// <summary>
    /// Creates a Fix64 from a fractional number.
    /// </summary>
    /// <param name="numerator">The numerator of the fraction.</param>
    /// <param name="denominator">The denominator of the fraction.</param>
    /// <returns>A Fix64 representing the fraction.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Fraction(double numerator, double denominator)
    {
        return new Fix64(numerator / denominator);
    }

    /// <summary>
    /// x++ (post-increment)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 PostIncrement(ref Fix64 a)
    {
        Fix64 originalValue = a;
        a._rawValue += One._rawValue;
        return originalValue;
    }

    /// <summary>
    /// x-- (post-decrement)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 PostDecrement(ref Fix64 a)
    {
        Fix64 originalValue = a;
        a._rawValue -= One._rawValue;
        return originalValue;
    }

    /// <summary>
    /// Counts the leading zeros in a 64-bit unsigned integer.
    /// </summary>
    /// <param name="x">The number to count leading zeros for.</param>
    /// <returns>The number of leading zeros.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int CountLeadingZeroes(ulong x)
    {
        int result = 0;
        while ((x & 0xF000000000000000) == 0) { result += 4; x <<= 4; }
        while ((x & 0x8000000000000000) == 0) { result += 1; x <<= 1; }
        return result;
    }

    /// <summary>
    /// Returns a number indicating the sign of a Fix64 number.
    /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(Fix64 value)
    {
        // Return the sign of the value, optimizing for branchless comparison
        return value._rawValue < 0 ? -1 : value._rawValue > 0 ? 1 : 0;
    }

    /// <summary>
    /// Returns true if the number has no decimal part (i.e., if the number is equivalent to an integer) and False otherwise. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInteger(Fix64 value)
    {
        return ((ulong)value._rawValue & MathFix.MAX_SHIFTED_AMOUNT_UI) == 0;
    }

    #endregion

    #region Explicit and Implicit Conversions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Fix64(long value)
    {
        return FromRaw(value << MathFix.SHIFT_AMOUNT_I);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator long(Fix64 value)
    {
        return value._rawValue >> MathFix.SHIFT_AMOUNT_I;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Fix64(int value)
    {
        return new Fix64(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator int(Fix64 value)
    {
        return (int)(value._rawValue >> MathFix.SHIFT_AMOUNT_I);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Fix64(float value)
    {
        return new Fix64((double)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator float(Fix64 value)
    {
        return value._rawValue * MathFix.SCALE_FACTOR_F;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Fix64(double value)
    {
        return new Fix64(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator double(Fix64 value)
    {
        return value._rawValue * MathFix.SCALE_FACTOR_D;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Fix64(decimal value)
    {
        return new Fix64((double)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator decimal(Fix64 value)
    {
        return value._rawValue * MathFix.SCALE_FACTOR_M;
    }

    #endregion

    #region Arithmetic Operators

    /// <summary>
    /// Adds two Fix64 numbers, with saturating behavior in case of overflow.
    /// </summary>
    public static Fix64 operator +(Fix64 x, Fix64 y)
    {
        long xl = x._rawValue;
        long yl = y._rawValue;
        long sum = xl + yl;
        // Check for overflow, if signs of operands are equal and signs of sum and x are different
        if ((~(xl ^ yl) & (xl ^ sum) & MathFix.MIN_VALUE_L) != 0)
            sum = xl > 0 ? MathFix.MAX_VALUE_L : MathFix.MIN_VALUE_L;
        return new Fix64(sum);
    }

    /// <summary>
    /// Adds an int to x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator +(Fix64 x, int y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D + y);
    }

    /// <summary>
    /// Adds an Fix64 to x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator +(int x, Fix64 y)
    {
        return y + x;
    }

    /// <summary>
    /// Adds a float to x 
    /// </summary>
    public static Fix64 operator +(Fix64 x, float y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D + y);
    }

    /// <summary>
    /// Adds a Fix64 to x 
    /// </summary>
    public static Fix64 operator +(float x, Fix64 y)
    {
        return y + x;
    }

    /// <summary>
    /// Subtracts one Fix64 number from another, with saturating behavior in case of overflow.
    /// </summary>
    public static Fix64 operator -(Fix64 x, Fix64 y)
    {
        long xl = x._rawValue;
        long yl = y._rawValue;
        long diff = xl - yl;
        // Check for overflow, if signs of operands are different and signs of sum and x are different
        if (((xl ^ yl) & (xl ^ diff) & MathFix.MIN_VALUE_L) != 0)
            diff = xl < 0 ? MathFix.MIN_VALUE_L : MathFix.MAX_VALUE_L;
        return new Fix64(diff);
    }

    /// <summary>
    /// Subtracts an int from x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator -(Fix64 x, int y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D - y);
    }

    /// <summary>
    /// Subtracts a Fix64 from x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator -(int x, Fix64 y)
    {
        return new Fix64(x - y._rawValue * MathFix.SCALE_FACTOR_D);
    }

    /// <summary>
    /// Subtracts a float from x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator -(Fix64 x, float y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D - y);
    }

    /// <summary>
    /// Subtracts a Fix64 from x 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator -(float x, Fix64 y)
    {
        return new Fix64(x - y._rawValue * MathFix.SCALE_FACTOR_D);
    }

    /// <summary>
    /// Multiplies two Fix64 numbers, handling overflow and rounding.
    /// </summary>
    public static Fix64 operator *(Fix64 x, Fix64 y)
    {
        long xl = x._rawValue;
        long yl = y._rawValue;

        // Split both numbers into high and low parts
        ulong xlo = (ulong)(xl & MathFix.MAX_SHIFTED_AMOUNT_UI);
        long xhi = xl >> MathFix.SHIFT_AMOUNT_I;
        ulong ylo = (ulong)(yl & MathFix.MAX_SHIFTED_AMOUNT_UI);
        long yhi = yl >> MathFix.SHIFT_AMOUNT_I;

        // Perform partial products
        ulong lolo = xlo * ylo;          // low bits * low bits
        long lohi = (long)xlo * yhi;     // low bits * high bits
        long hilo = xhi * (long)ylo;     // high bits * low bits
        long hihi = xhi * yhi;           // high bits * high bits

        // Combine results, starting with the low part
        ulong loResult = lolo >> MathFix.SHIFT_AMOUNT_I;
        long hiResult = hihi << MathFix.SHIFT_AMOUNT_I;

        // Adjust rounding for the fractional part of the lolo term
        if ((lolo & 1UL << MathFix.SHIFT_AMOUNT_I - 1) != 0)
            loResult++; // Apply rounding up if the dropped bit is 1 (round half-up)

        bool overflow = false;
        long sum = MathFix.AddOverflowHelper((long)loResult, lohi, ref overflow);
        sum = MathFix.AddOverflowHelper(sum, hilo, ref overflow);
        sum = MathFix.AddOverflowHelper(sum, hiResult, ref overflow);

        // Overflow handling
        bool opSignsEqual = ((xl ^ yl) & MathFix.MIN_VALUE_L) == 0;

        // Positive overflow check
        if (opSignsEqual)
        {
            if (sum < 0 || overflow && xl > 0)
                return MAX_VALUE;
        }
        else
        {
            if (sum > 0)
                return MIN_VALUE;
        }

        // Final overflow check: if the high 32 bits are non-zero or non-sign-extended, it's an overflow
        long topCarry = hihi >> MathFix.SHIFT_AMOUNT_I;
        if (topCarry != 0 && topCarry != -1)
            return opSignsEqual ? MAX_VALUE : MIN_VALUE;

        // Negative overflow check
        if (!opSignsEqual)
        {
            long posOp = xl > yl ? xl : yl;
            long negOp = xl < yl ? xl : yl;

            if (sum > negOp && negOp < -MathFix.ONE_L && posOp > MathFix.ONE_L)
                return MIN_VALUE;
        }

        return new Fix64(sum);
    }

    /// <summary>
    /// Multiplies a Fix64 by an integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator *(Fix64 x, int y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D * y);
    }

    /// <summary>
    /// Multiplies an integer by a 
    /// </summary>
    public static Fix64 operator *(int x, Fix64 y)
    {
        return y * x;
    }

    /// <summary>
    /// Divides one Fix64 number by another, handling division by zero and overflow.
    /// </summary>
    public static Fix64 operator /(Fix64 x, Fix64 y)
    {
        long xl = x._rawValue;
        long yl = y._rawValue;

        if (yl == 0)
            throw new DivideByZeroException($"Attempted to divide {x} by zero.");

        ulong remainder = (ulong)(xl < 0 ? -xl : xl);
        ulong divider = (ulong)(yl < 0 ? -yl : yl);
        ulong quotient = 0UL;
        int bitPos = MathFix.SHIFT_AMOUNT_I + 1;

        // If the divider is divisible by 2^n, take advantage of it.
        while ((divider & 0xF) == 0 && bitPos >= 4)
        {
            divider >>= 4;
            bitPos -= 4;
        }

        while (remainder != 0 && bitPos >= 0)
        {
            int shift = CountLeadingZeroes(remainder);
            if (shift > bitPos)
                shift = bitPos;

            remainder <<= shift;
            bitPos -= shift;

            ulong div = remainder / divider;
            remainder %= divider;
            quotient += div << bitPos;

            // Detect overflow
            if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                return ((xl ^ yl) & MathFix.MIN_VALUE_L) == 0 ? MAX_VALUE : MIN_VALUE;

            remainder <<= 1;
            --bitPos;
        }

        // Rounding logic: "Round half to even" or "Banker's rounding"
        if ((quotient & 0x1) != 0)
            quotient += 1;

        long result = (long)(quotient >> 1);
        if (((xl ^ yl) & MathFix.MIN_VALUE_L) != 0)
            result = -result;

        return new Fix64(result);
    }

    /// <summary>
    /// Divides a Fix64 by an integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator /(Fix64 x, int y)
    {
        return new Fix64(x._rawValue * MathFix.SCALE_FACTOR_D / y);
    }

    /// <summary>
    /// Computes the remainder of division of one Fix64 number by another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator %(Fix64 x, Fix64 y)
    {
        if (x._rawValue == MathFix.MIN_VALUE_L && y._rawValue == -1)
            return Zero;
        return new Fix64(x._rawValue % y._rawValue);
    }

    /// <summary>
    /// Unary negation operator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator -(Fix64 x)
    {
        return x._rawValue == MathFix.MIN_VALUE_L ? MAX_VALUE : new Fix64(-x._rawValue);
    }

    /// <summary>
    /// Pre-increment operator (++x).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator ++(Fix64 a)
    {
        a._rawValue += One._rawValue;
        return a;
    }

    /// <summary>
    /// Pre-decrement operator (--x).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator --(Fix64 a)
    {
        a._rawValue -= One._rawValue;
        return a;
    }

    /// <summary>
    /// Bitwise left shift operator.
    /// </summary>
    /// <param name="a">Operand to shift.</param>
    /// <param name="shift">Number of bits to shift.</param>
    /// <returns>The shifted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator <<(Fix64 a, int shift)
    {
        return new Fix64(a._rawValue << shift);
    }

    /// <summary>
    /// Bitwise right shift operator.
    /// </summary>
    /// <param name="a">Operand to shift.</param>
    /// <param name="shift">Number of bits to shift.</param>
    /// <returns>The shifted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 operator >>(Fix64 a, int shift)
    {
        return new Fix64(a._rawValue >> shift);
    }

    #endregion

    #region Comparison Operators

    /// <summary>
    /// Determines whether one Fix64 is greater than another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Fix64 x, Fix64 y)
    {
        return x._rawValue > y._rawValue;
    }

    /// <summary>
    /// Determines whether one Fix64 is less than another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Fix64 x, Fix64 y)
    {
        return x._rawValue < y._rawValue;
    }

    /// <summary>
    /// Determines whether one Fix64 is greater than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Fix64 x, Fix64 y)
    {
        return x._rawValue >= y._rawValue;
    }

    /// <summary>
    /// Determines whether one Fix64 is less than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Fix64 x, Fix64 y)
    {
        return x._rawValue <= y._rawValue;
    }

    /// <summary>
    /// Determines whether two Fix64 instances are equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Fix64 left, Fix64 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two Fix64 instances are not equal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Fix64 left, Fix64 right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Conversion

    /// <summary>
    /// Returns the string representation of this Fix64 instance.
    /// </summary>
    /// <remarks>
    /// Up to 10 decimal places.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return ((double)this).ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts the numeric value of the current Fix64 object to its equivalent string representation.
    /// </summary>
    /// <param name="format">A format specification that governs how the current Fix64 object is converted.</param>
    /// <returns>The string representation of the value of the current Fix64 object.</returns>  
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string format)
    {
        return ((double)this).ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Parses a string to create a Fix64 instance.
    /// </summary>
    /// <param name="s">The string representation of the </param>
    /// <returns>The parsed Fix64 value.</returns>
    public static Fix64 Parse(string s)
    {
        if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

        // Check if the value is negative
        bool isNegative = false;
        if (s[0] == '-')
        {
            isNegative = true;
            s = s[1..];
        }

        if (!long.TryParse(s, out long rawValue))
            throw new FormatException($"Invalid format: {s}");

        // If the value was negative, negate the result
        if (isNegative)
            rawValue = -rawValue;

        return FromRaw(rawValue);
    }

    /// <summary>
    /// Tries to parse a string to create a Fix64 instance.
    /// </summary>
    /// <param name="s">The string representation of the </param>
    /// <param name="result">The parsed Fix64 value.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string s, out Fix64 result)
    {
        result = Zero;
        if (string.IsNullOrEmpty(s)) return false;

        // Check if the value is negative
        bool isNegative = false;
        if (s[0] == '-')
        {
            isNegative = true;
            s = s[1..];
        }

        if (!long.TryParse(s, out long rawValue)) return false;

        // If the value was negative, negate the result
        if (isNegative)
            rawValue = -rawValue;

        result = FromRaw(rawValue);
        return true;
    }

    /// <summary>
    /// Creates a Fix64 from a raw long value.
    /// </summary>
    /// <param name="rawValue">The raw long value.</param>
    /// <returns>A Fix64 representing the raw value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 FromRaw(long rawValue)
    {
        return new Fix64(rawValue);
    }

    /// <summary>
    /// Converts a Fix64s RawValue (Int64) into a double
    /// </summary>
    /// <param name="f1"></param>
    /// <returns></returns>
    public static double ToDouble(long f1)
    {
        return f1 * MathFix.SCALE_FACTOR_D;
    }

    /// <summary>
    /// Converts a Fix64s RawValue (Int64) into a float
    /// </summary>
    /// <param name="f1"></param>
    /// <returns></returns>
    public static float ToFloat(long f1)
    {
        return f1 * MathFix.SCALE_FACTOR_F;
    }

    /// <summary>
    /// Converts a Fix64s RawValue (Int64) into a decimal
    /// </summary>
    /// <param name="f1"></param>
    /// <returns></returns>
    public static decimal ToDecimal(long f1)
    {
        return f1 * MathFix.SCALE_FACTOR_M;
    }

    #endregion

    #region Equality, HashCode, Comparable Overrides

    /// <summary>
    /// Determines whether this instance equals another object.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is Fix64 other && Equals(other);
    }

    /// <summary>
    /// Determines whether this instance equals another 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Fix64 other)
    {
        return _rawValue == other._rawValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Fix64 x, Fix64 y)
    {
        return x.Equals(y);
    }

    /// <summary>
    /// Returns the hash code for this Fix64 instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return _rawValue.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetHashCode(Fix64 obj)
    {
        return obj.GetHashCode();
    }

    /// <summary>
    /// Compares this instance to another 
    /// </summary>
    /// <param name="other">The Fix64 to compare with.</param>
    /// <returns>-1 if less than, 0 if equal, 1 if greater than other.</returns>
    public readonly int CompareTo(Fix64 other)
    {
        return _rawValue.CompareTo(other._rawValue);
    }

    #endregion
}