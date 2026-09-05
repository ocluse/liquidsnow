namespace Ocluse.LiquidSnow.Numerics;

public static partial class MathFix
{
    private const decimal DecimalPI = 3.1415926535897932384626433833m;

    private static decimal AtanDecimal(decimal value)
    {
        bool negative = value < 0;
        decimal x = decimal.Abs(value);
        bool reciprocal = x > 1;
        if (reciprocal) x = 1 / x;
        bool shifted = x > 0.4142135623730950488m;
        if (shifted) x = (x - 1) / (x + 1);
        decimal term = x, result = x, square = x * x;
        for (int i = 3; i <= 65; i += 2)
        {
            term *= -square;
            result += term / i;
        }
        if (shifted) result += DecimalPI / 4;
        if (reciprocal) result = DecimalPI / 2 - result;
        return negative ? -result : result;
    }

    // Restoring integer square root. The remaining input is the exact remainder.
    internal static UInt128 SqrtRounded(UInt128 input)
    {
        UInt128 result = 0, bit = (UInt128)1 << 126;
        while (bit > input) bit >>= 2;
        while (bit != 0)
        {
            if (input >= result + bit)
            {
                input -= result + bit;
                result = (result >> 1) + bit;
            }
            else result >>= 1;
            bit >>= 2;
        }
        return input > result ? result + 1 : result;
    }

    // Accumulate unscaled squares, clamping only when the final length cannot fit.
    internal static Fix64 WideMagnitude(Int128 x, Int128 y, Int128 z = default, Int128 w = default)
    {
        UInt128 limit = (UInt128)long.MaxValue * (UInt128)long.MaxValue;
        UInt128 sum = 0;
        ReadOnlySpan<Int128> components = stackalloc Int128[] { x, y, z, w };
        foreach (Int128 component in components)
        {
            UInt128 magnitude = (UInt128)Int128.Abs(component);
            if (magnitude > (UInt128)long.MaxValue) return Fix64.MAX_VALUE;
            UInt128 square = magnitude * magnitude;
            if (square > limit - sum) return Fix64.MAX_VALUE;
            sum += square;
        }
        return Fix64.FromRaw((long)SqrtRounded(sum));
    }

    internal static bool WithinDistance(FixVector3 a, FixVector3 b, Int128 radiusRaw)
    {
        if (radiusRaw < 0) return false;
        UInt128 remaining = (UInt128)radiusRaw * (UInt128)radiusRaw;
        ReadOnlySpan<Int128> components = stackalloc Int128[]
        {
            (Int128)a.X.RawValue - b.X.RawValue,
            (Int128)a.Y.RawValue - b.Y.RawValue,
            (Int128)a.Z.RawValue - b.Z.RawValue
        };
        foreach (Int128 component in components)
        {
            UInt128 magnitude = (UInt128)Int128.Abs(component);
            UInt128 square = magnitude * magnitude;
            if (square > remaining) return false;
            remaining -= square;
        }
        return true;
    }
}
