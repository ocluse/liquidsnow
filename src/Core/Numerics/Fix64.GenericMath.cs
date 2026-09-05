#pragma warning disable CS1591

using System.Globalization;
using System.Numerics;

namespace Ocluse.LiquidSnow.Numerics;

// Q32.32 arithmetic saturates by default; checked arithmetic throws on overflow.
// Quantization uses nearest-even rounding. There are no NaN or infinity values.
public partial struct Fix64 : INumber<Fix64>, IMinMaxValue<Fix64>, ISignedNumber<Fix64>
{
    static Fix64 INumberBase<Fix64>.Zero => Zero;
    static Fix64 INumberBase<Fix64>.One => One;
    public static Fix64 NegativeOne => -One;
    public static Fix64 MinValue => MIN_VALUE;
    public static Fix64 MaxValue => MAX_VALUE;
    public static Fix64 AdditiveIdentity => Zero;
    public static Fix64 MultiplicativeIdentity => One;
    public static int Radix => 2;

    internal static Fix64 FromWide(Int128 raw) => FromRaw((long)Int128.Clamp(raw, long.MinValue, long.MaxValue));
    internal static Fix64 FromWideChecked(Int128 raw) => FromRaw(checked((long)raw));

    // Equivalent to DivideRounded(product, MathFix.ONE_L) without the 128-bit division.
    internal static Int128 ShiftRounded(Int128 product)
    {
        Int128 quotient = product >> MathFix.SHIFT_AMOUNT_I;
        uint remainder = (uint)(product & MathFix.MAX_SHIFTED_AMOUNT_UI);
        const uint half = 1u << (MathFix.SHIFT_AMOUNT_I - 1);
        if (remainder > half || (remainder == half && (quotient & 1) != 0))
            quotient++;
        return quotient;
    }

    internal static Int128 DivideRounded(Int128 numerator, Int128 denominator)
    {
        if (denominator == 0) throw new DivideByZeroException();
        Int128 quotient = numerator / denominator;
        Int128 remainder = numerator % denominator;
        Int128 twiceRemainder = Int128.Abs(remainder) * 2;
        Int128 divisor = Int128.Abs(denominator);
        if (twiceRemainder > divisor || (twiceRemainder == divisor && (quotient & 1) != 0))
            quotient += (numerator < 0) == (denominator < 0) ? 1 : -1;
        return quotient;
    }

    internal static Fix64 FromDecimal(decimal value, bool saturating = false)
    {
        if (value < (decimal)MIN_VALUE || value > (decimal)MAX_VALUE)
        {
            if (!saturating) throw new OverflowException("Value is outside the Q32.32 range.");
            return value < 0 ? MIN_VALUE : MAX_VALUE;
        }
        return FromRaw((long)decimal.Round(value * MathFix.ONE_L, 0, MidpointRounding.ToEven));
    }

    private static Fix64 FromDouble(double value, bool saturating = false)
    {
        if (double.IsNaN(value))
        {
            if (saturating) return Zero;
            throw new OverflowException("Fix64 cannot represent NaN.");
        }
        // Test the scaled double, rather than converting through decimal (which loses binary bits).
        double raw = Math.Round(value * MathFix.ONE_L, MidpointRounding.ToEven);
        if (raw >= 9223372036854775808d || raw < -9223372036854775808d)
        {
            if (saturating) return value < 0 ? MIN_VALUE : MAX_VALUE;
            throw new OverflowException("Value is outside the Q32.32 range.");
        }
        return FromRaw(checked((long)raw));
    }

    public static Fix64 operator +(Fix64 value) => value;
    public static Fix64 operator checked +(Fix64 x, Fix64 y) => FromWideChecked((Int128)x.RawValue + y.RawValue);
    public static Fix64 operator checked -(Fix64 x, Fix64 y) => FromWideChecked((Int128)x.RawValue - y.RawValue);
    public static Fix64 operator checked *(Fix64 x, Fix64 y) => FromWideChecked(ShiftRounded((Int128)x.RawValue * y.RawValue));
    public static Fix64 operator checked /(Fix64 x, Fix64 y) => FromWideChecked(DivideRounded((Int128)x.RawValue << 32, y.RawValue));
    public static Fix64 operator checked -(Fix64 value) => FromWideChecked(-(Int128)value.RawValue);
    public static Fix64 operator checked ++(Fix64 value) => checked(value + One);
    public static Fix64 operator checked --(Fix64 value) => checked(value - One);
    public static Fix64 operator checked +(Fix64 x, int y) => checked(x + (Fix64)y);
    public static Fix64 operator checked +(int x, Fix64 y) => checked((Fix64)x + y);
    public static Fix64 operator checked -(Fix64 x, int y) => checked(x - (Fix64)y);
    public static Fix64 operator checked -(int x, Fix64 y) => checked((Fix64)x - y);
    public static Fix64 operator checked *(Fix64 x, int y) => FromWideChecked((Int128)x.RawValue * y);
    public static Fix64 operator checked *(int x, Fix64 y) => checked(y * x);
    public static Fix64 operator checked /(Fix64 x, int y) => FromWideChecked(DivideRounded(x.RawValue, y));

    public static Fix64 Abs(Fix64 value) => MathFix.Abs(value);
    public static Fix64 Clamp(Fix64 value, Fix64 min, Fix64 max)
    {
        if (min > max) throw new ArgumentException("Minimum exceeds maximum.");
        return MathFix.Clamp(value, min, max);
    }
    public static Fix64 CopySign(Fix64 value, Fix64 sign) => MathFix.CopySign(value, sign);
    public static Fix64 Max(Fix64 x, Fix64 y) => x >= y ? x : y;
    public static Fix64 Min(Fix64 x, Fix64 y) => x <= y ? x : y;
    public static Fix64 MaxNumber(Fix64 x, Fix64 y) => Max(x, y);
    public static Fix64 MinNumber(Fix64 x, Fix64 y) => Min(x, y);
    public static Fix64 MaxMagnitude(Fix64 x, Fix64 y)
    {
        Int128 a = Int128.Abs(x.RawValue), b = Int128.Abs(y.RawValue);
        return a == b ? Max(x, y) : a > b ? x : y;
    }
    public static Fix64 MinMagnitude(Fix64 x, Fix64 y)
    {
        Int128 a = Int128.Abs(x.RawValue), b = Int128.Abs(y.RawValue);
        return a == b ? Min(x, y) : a < b ? x : y;
    }
    public static Fix64 MaxMagnitudeNumber(Fix64 x, Fix64 y) => MaxMagnitude(x, y);
    public static Fix64 MinMagnitudeNumber(Fix64 x, Fix64 y) => MinMagnitude(x, y);
    public static bool IsCanonical(Fix64 value) => true;
    public static bool IsComplexNumber(Fix64 value) => false;
    public static bool IsEvenInteger(Fix64 value) => IsInteger(value) && (value.RawValue & MathFix.ONE_L) == 0;
    public static bool IsFinite(Fix64 value) => true;
    public static bool IsImaginaryNumber(Fix64 value) => false;
    public static bool IsInfinity(Fix64 value) => false;
    public static bool IsNaN(Fix64 value) => false;
    public static bool IsNegative(Fix64 value) => value.RawValue < 0;
    public static bool IsNegativeInfinity(Fix64 value) => false;
    public static bool IsNormal(Fix64 value) => value.RawValue != 0;
    public static bool IsOddInteger(Fix64 value) => IsInteger(value) && (value.RawValue & MathFix.ONE_L) != 0;
    public static bool IsPositive(Fix64 value) => value.RawValue >= 0;
    public static bool IsPositiveInfinity(Fix64 value) => false;
    public static bool IsRealNumber(Fix64 value) => true;
    public static bool IsSubnormal(Fix64 value) => false;
    public static bool IsZero(Fix64 value) => value.RawValue == 0;
    public readonly int CompareTo(object? obj) => obj is null ? 1 : obj is Fix64 other ? CompareTo(other) : throw new ArgumentException("Expected Fix64.", nameof(obj));

    public readonly string ToString(string? format, IFormatProvider? provider) => ((decimal)this).ToString(format, provider);
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((decimal)this).TryFormat(destination, out charsWritten, format, provider);

    public static Fix64 Parse(string s, IFormatProvider? provider) => Parse(s, NumberStyles.Number | NumberStyles.AllowExponent, provider);
    public static Fix64 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, NumberStyles.Number | NumberStyles.AllowExponent, provider);
    public static Fix64 Parse(string s, NumberStyles style, IFormatProvider? provider) => FromDecimal(decimal.Parse(s, style, provider));
    public static Fix64 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => FromDecimal(decimal.Parse(s, style, provider));
    public static bool TryParse(string? s, IFormatProvider? provider, out Fix64 result) => TryParse(s.AsSpan(), provider, out result);
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fix64 result) => TryParse(s, NumberStyles.Number | NumberStyles.AllowExponent, provider, out result);
    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out Fix64 result) => TryParse(s.AsSpan(), style, provider, out result);
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fix64 result)
    {
        result = Zero;
        if (!decimal.TryParse(s, style, provider, out decimal value) || value < (decimal)MIN_VALUE || value > (decimal)MAX_VALUE) return false;
        result = FromDecimal(value);
        return true;
    }
    /// <summary>Parses an invariant signed integer containing the underlying raw bits.</summary>
    public static Fix64 ParseRaw(string s) => FromRaw(long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture));
    public static bool TryParseRaw(string? s, out Fix64 result)
    {
        bool success = long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out long raw);
        result = FromRaw(raw);
        return success;
    }

    public static Fix64 CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther> => ConvertFrom(value, false);
    public static Fix64 CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther> => ConvertFrom(value, true);
    // Like other fractional numeric types, truncating conversion clamps range overflow.
    public static Fix64 CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther> => ConvertFrom(value, true);
    private static Fix64 ConvertFrom<TOther>(TOther value, bool saturating) where TOther : INumberBase<TOther>
    {
        if (value is Fix64 f) return f;
        if (value is double d) return FromDouble(d, saturating);
        if (value is float s) return FromDouble(s, saturating);
        if (value is Half h) return FromDouble((double)h, saturating);
        return FromDecimal(saturating ? decimal.CreateSaturating(value) : decimal.CreateChecked(value), saturating);
    }
    private static bool TryConvertFrom<TOther>(TOther value, bool saturating, out Fix64 result) where TOther : INumberBase<TOther>
    {
        try { result = ConvertFrom(value, saturating); return true; }
        catch (NotSupportedException) { result = Zero; return false; }
    }
    static bool INumberBase<Fix64>.TryConvertFromChecked<TOther>(TOther value, out Fix64 result) => TryConvertFrom(value, false, out result);
    static bool INumberBase<Fix64>.TryConvertFromSaturating<TOther>(TOther value, out Fix64 result) => TryConvertFrom(value, true, out result);
    static bool INumberBase<Fix64>.TryConvertFromTruncating<TOther>(TOther value, out Fix64 result) => TryConvertFrom(value, true, out result);
    private static bool TryConvertTo<TOther>(Fix64 value, int mode, out TOther result) where TOther : INumberBase<TOther>
    {
        try
        {
            result = mode switch
            {
                0 => TOther.CreateChecked((decimal)value),
                1 => TOther.CreateSaturating((decimal)value),
                _ => TOther.CreateTruncating((decimal)value)
            };
            return true;
        }
        catch (NotSupportedException) { result = default!; return false; }
    }
    static bool INumberBase<Fix64>.TryConvertToChecked<TOther>(Fix64 value, out TOther result) => TryConvertTo(value, 0, out result);
    static bool INumberBase<Fix64>.TryConvertToSaturating<TOther>(Fix64 value, out TOther result) => TryConvertTo(value, 1, out result);
    static bool INumberBase<Fix64>.TryConvertToTruncating<TOther>(Fix64 value, out TOther result) => TryConvertTo(value, 2, out result);
}
