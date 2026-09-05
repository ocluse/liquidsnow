using System.Globalization;
using System.Numerics;
using System.Text.Json;
using Ocluse.LiquidSnow.Numerics;
using Ocluse.LiquidSnow.Numerics.Extensions;
using Ocluse.LiquidSnow.Numerics.Bounds;

namespace Ocluse.LiquidSnow.Core.Tests.Numerics;

public class FixedMathTests
{
    private static T GenericSum<T>(IEnumerable<T> values) where T : INumber<T>
    {
        T result = T.Zero;
        foreach (T value in values) result += value;
        return result / T.One;
    }

    private static long Rounded(BigInteger numerator, BigInteger denominator)
    {
        BigInteger q = BigInteger.DivRem(numerator, denominator, out BigInteger r);
        int comparison = (BigInteger.Abs(r) * 2).CompareTo(BigInteger.Abs(denominator));
        if (comparison > 0 || comparison == 0 && !q.IsEven) q += numerator.Sign * denominator.Sign;
        return (long)BigInteger.Clamp(q, long.MinValue, long.MaxValue);
    }

    [Fact]
    public void ArithmeticMatchesBigIntegerOracle()
    {
        long[] edges = [long.MinValue, long.MinValue + 1, -4294967296, -3, -1, 0, 1, 3, 4294967296, long.MaxValue - 1, long.MaxValue];
        var random = new Random(7182);
        byte[] bytes = new byte[8];
        long Next() { random.NextBytes(bytes); return BitConverter.ToInt64(bytes); }
        void Check(long a, long b)
        {
            Fix64 x = Fix64.FromRaw(a), y = Fix64.FromRaw(b);
            Assert.Equal((long)BigInteger.Clamp((BigInteger)a + b, long.MinValue, long.MaxValue), (x + y).RawValue);
            Assert.Equal((long)BigInteger.Clamp((BigInteger)a - b, long.MinValue, long.MaxValue), (x - y).RawValue);
            Assert.Equal(Rounded((BigInteger)a * b, 1L << 32), (x * y).RawValue);
            if (b != 0) Assert.Equal(Rounded((BigInteger)a << 32, b), (x / y).RawValue);
        }
        foreach (long a in edges) foreach (long b in edges) Check(a, b);
        for (int i = 0; i < 5000; i++) Check(Next(), Next());
    }

    [Fact]
    public void IntegerOverloadsPreserveBitsAndCheckZero()
    {
        Fix64 x = Fix64.FromRaw(9007199254740993);
        Assert.Equal(x, x + 0);
        Assert.Equal(x, x - 0);
        Assert.Equal(x, x * 1);
        Assert.Equal(x, x / 1);
        Assert.Equal(x + (Fix64)17, x + 17);
        Assert.Equal(x * (Fix64)17, x * 17);
        Assert.Equal(x / (Fix64)17, x / 17);
        Assert.Throws<DivideByZeroException>(() => x / 0);
        Assert.Throws<DivideByZeroException>(() => x / Fix64.Zero);
        Assert.Throws<DivideByZeroException>(() => Fix64.Fraction(1, 0));
        Fix64 max = Fix64.MaxValue, min = Fix64.MinValue;
        Assert.Equal(max, ++max);
        Assert.Equal(min, --min);
        Assert.Throws<OverflowException>(() => checked(max + Fix64.One));
        Assert.Throws<OverflowException>(() => checked(min / -Fix64.One));
        Assert.Throws<OverflowException>(() => checked(max * Fix64.Two));
        Assert.Throws<OverflowException>(() => checked(max + 1));
        Assert.Throws<OverflowException>(() => checked(max * 2));
    }

    [Fact]
    public void GenericMathSupportsIdentitiesClassificationsAndConversions()
    {
        Assert.Equal((Fix64)6, GenericSum<Fix64>([(Fix64)1, (Fix64)2, (Fix64)3]));
        Assert.Equal((Fix64)1.5m, Fix64.CreateChecked(1.5m));
        Assert.Equal(Fix64.MaxValue, Fix64.CreateSaturating(BigInteger.One << 200));
        Assert.Equal(Fix64.MinValue, Fix64.CreateTruncating(-(BigInteger.One << 200)));
        Assert.Throws<OverflowException>(() => Fix64.CreateChecked(BigInteger.One << 200));
        Assert.Throws<OverflowException>(() => Fix64.CreateChecked(double.NaN));
        Assert.Throws<OverflowException>(() => Fix64.CreateChecked(double.PositiveInfinity));
        Assert.Equal(Fix64.Zero, Fix64.CreateSaturating(double.NaN));
        Assert.Equal(Fix64.MaxValue, Fix64.CreateSaturating(double.PositiveInfinity));
        Assert.Equal(1.5m, decimal.CreateChecked((Fix64)1.5));
        Assert.Equal(-1, int.CreateChecked((Fix64)(-1.5)));
        Assert.Equal(-1, (int)(Fix64)(-1.5));
        Assert.Throws<OverflowException>(() => byte.CreateChecked((Fix64)300));
        Assert.Equal(byte.MaxValue, byte.CreateSaturating((Fix64)300));
        // Fractional-source conversions follow decimal's clamping semantics.
        Assert.Equal(byte.MaxValue, byte.CreateTruncating((Fix64)300));
        Assert.True(Fix64.IsEvenInteger((Fix64)(-2)));
        Assert.True(Fix64.IsOddInteger((Fix64)(-3)));
        Assert.False(Fix64.IsInteger(Fix64.Half));
        Assert.Equal(Fix64.MinValue, Fix64.MaxMagnitude(Fix64.MinValue, Fix64.MaxValue));
        Assert.Equal(-Fix64.One, Fix64.MinMagnitude(-Fix64.One, Fix64.One));
        Assert.Equal(Fix64.MinValue, Fix64.CopySign(Fix64.MinValue, -Fix64.One));
    }

    [Fact]
    public void TextDecimalAndJsonRoundTripEveryRawBit()
    {
        var random = new Random(829);
        long[] edges = [long.MinValue, long.MaxValue, 1, -1, 9007199254740993];
        Span<char> text = stackalloc char[64];
        foreach (long raw in edges.Concat(Enumerable.Range(0, 1000).Select(_ => random.NextInt64(long.MinValue, long.MaxValue))))
        {
            Fix64 value = Fix64.FromRaw(raw);
            Assert.Equal(value, Fix64.Parse(value.ToString()));
            Assert.Equal(value, (Fix64)(decimal)value);
            Assert.Equal(value, JsonSerializer.Deserialize<Fix64>(JsonSerializer.Serialize(value)));
            Assert.Equal(value, Fix64.ParseRaw(value.RawToString()));
            Assert.True(value.TryFormat(text, out int count, default, CultureInfo.InvariantCulture));
            Assert.Equal(value, Fix64.Parse(text[..count], CultureInfo.InvariantCulture));
        }
        var french = CultureInfo.GetCultureInfo("fr-FR");
        Assert.Equal((Fix64)1.25, Fix64.Parse("1,25", french));
        Assert.Equal((Fix64)100, Fix64.Parse("1e2"));
        Assert.False(Fix64.TryParse("--1", out _));
        Assert.False(Fix64.TryParse("2147483648", out _));
        Assert.False(Fix64.TryParseRaw("--1", out _));
        Assert.True(Fix64.TryParseRaw(long.MinValue.ToString(CultureInfo.InvariantCulture), out var min));
        Assert.Equal(Fix64.MinValue, min);
    }

    [Theory]
    [InlineData(-1.5, MidpointRounding.AwayFromZero, -2)]
    [InlineData(-2.5, MidpointRounding.ToEven, -2)]
    [InlineData(1.9, MidpointRounding.ToZero, 1)]
    [InlineData(-1.1, MidpointRounding.ToNegativeInfinity, -2)]
    [InlineData(-1.9, MidpointRounding.ToPositiveInfinity, -1)]
    public void RoundHonorsModes(double value, MidpointRounding mode, int expected)
        => Assert.Equal((Fix64)expected, MathFix.Round((Fix64)value, mode));

    [Fact]
    public void DecimalRoundingDoesNotOverflowDuringScaling()
    {
        Assert.Equal((Fix64)100, MathFix.RoundToPrecision((Fix64)100, 9));
        Assert.Equal(Fix64.MaxValue, MathFix.Round(Fix64.MaxValue));
        Assert.Equal(Fix64.Zero, MathFix.RoundToPrecision(Fix64.Zero, 9));
        Assert.Throws<ArgumentException>(() => MathFix.Round(Fix64.One, (MidpointRounding)999));
    }

    [Fact]
    public void TranscendentalsMeetAccuracyAndBoundaryExpectations()
    {
        for (int i = -32; i <= 30; i++) Assert.Equal((Fix64)Math.Pow(2, i), MathFix.Pow2((Fix64)i));
        Assert.Equal(Fix64.MaxValue, MathFix.Pow2((Fix64)31));
        Assert.Equal(Fix64.MaxValue, MathFix.Pow2((Fix64)32));
        Assert.Equal(Fix64.Zero, MathFix.Pow2((Fix64)(-33)));
        Assert.Equal(Fix64.Zero, MathFix.Pow2(Fix64.MinValue));
        Assert.InRange(Math.Abs((double)MathFix.Ln(Fix64.Two) - Math.Log(2)), 0, 1e-8);
        for (int i = -1000; i <= 1000; i++)
        {
            Fix64 angle = (Fix64)(i / 100.0);
            Assert.InRange(Math.Abs((double)MathFix.Sin(angle) - Math.Sin((double)angle)), 0, 2e-9);
            Assert.InRange(Math.Abs((double)MathFix.Cos(angle) - Math.Cos((double)angle)), 0, 2e-9);
            Assert.InRange(Math.Abs((double)MathFix.Atan(angle) - Math.Atan((double)angle)), 0, 3e-10);
            Fix64 unit = (Fix64)(i / 1000.0);
            Assert.InRange(Math.Abs((double)MathFix.Asin(unit) - Math.Asin((double)unit)), 0, 5e-9);
            Assert.InRange(Math.Abs((double)MathFix.Acos(unit) - Math.Acos((double)unit)), 0, 5e-9);
        }
        Assert.InRange(Math.Abs((double)MathFix.Atan2(Fix64.MaxValue, Fix64.Precision) - Math.PI / 2), 0, 3e-10);
        Assert.InRange(Math.Abs((double)MathFix.Sin(MathFix.PiOver2 - Fix64.Precision) - 1), 0, 3e-10);
        foreach (Fix64 large in new[] { (Fix64)1000000, (Fix64)(-1000000), (Fix64)int.MaxValue, Fix64.MinValue })
        {
            Assert.InRange(Math.Abs((double)MathFix.Sin(large) - Math.Sin((double)large)), 0, 3e-10);
            Assert.InRange(Math.Abs((double)MathFix.Cos(large) - Math.Cos((double)large)), 0, 3e-10);
        }
        foreach (long offset in new long[] { 1, 2, 10, 1000, 10000 })
        {
            Fix64 nearOne = Fix64.One - Fix64.FromRaw(offset);
            Assert.InRange(Math.Abs((double)MathFix.Asin(nearOne) - Math.Asin((double)nearOne)), 0, 4e-10);
            Assert.InRange(Math.Abs((double)MathFix.Acos(nearOne) - Math.Acos((double)nearOne)), 0, 4e-10);
        }
        Assert.Throws<DivideByZeroException>(() => MathFix.Tan(MathFix.PiOver2));
        Assert.Equal(Fix64.MaxValue, MathFix.RadToDeg(Fix64.MaxValue));
    }

    [Fact]
    public void SquareRootIsNearestRepresentableValue()
    {
        var random = new Random(719);
        foreach (long raw in new long[] { 0, 1, 2, 3, long.MaxValue }.Concat(Enumerable.Range(0, 1000).Select(_ => random.NextInt64(long.MaxValue))))
        {
            BigInteger n = (BigInteger)raw << 32;
            BigInteger r = MathFix.Sqrt(Fix64.FromRaw(raw)).RawValue;
            if (r > 0) Assert.True(4 * n >= BigInteger.Pow(2 * r - 1, 2));
            Assert.True(4 * n <= BigInteger.Pow(2 * r + 1, 2));
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => MathFix.Sqrt(-Fix64.One));
    }

    [Fact]
    public void GeometryPreservesLengthsAndRejectsDisjointRanges()
    {
        Assert.Equal(new FixVector3(2, 0, 0), FixQuaternion.Identity.Rotate(new FixVector3(2, 0, 0)));
        var q = new FixQuaternion(Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero);
        Assert.True(q.FuzzyEqual(q));
        Assert.Equal((Fix64)50000, new FixVector3(50000, 0, 0).Magnitude);
        Assert.Equal((Fix64)100000, FixVector3.Distance(new FixVector3(-50000, 0, 0), new FixVector3(50000, 0, 0)));
        Assert.Equal(FixVector3.Right, new FixVector3(Fix64.Precision, Fix64.Zero, Fix64.Zero).Normal);
        Assert.InRange((double)new FixVector3(Fix64.MaxValue, Fix64.MaxValue, Fix64.MaxValue).Normal.Magnitude, 0.999999999, 1.000000001);
        Assert.Equal((Fix64)50000, new FixVector2(50000, 0).Magnitude);
        var sphere = new FixBoundingSphere(FixVector3.Zero, (Fix64)50000);
        Assert.False(sphere.Contains(new FixVector3(60000, 0, 0)));
        Assert.False(sphere.Intersects(new FixBoundingSphere(new FixVector3(110000, 0, 0), (Fix64)50000)));
        Assert.True(sphere.Intersects(new FixBoundingSphere(new FixVector3(100000, 0, 0), (Fix64)50000)));
        Assert.False(FixRange.CheckOverlap(FixVector3.Right, new((Fix64)0, (Fix64)1), new((Fix64)2, (Fix64)3), (Fix64)10, Fix64.One, out _));
        Assert.Equal((Fix64)1500000000, new FixRange((Fix64)1400000000, (Fix64)1600000000).MidPoint);
        Assert.Equal((Fix64)2, FixRange.ComputeOverlapDepth(new((Fix64)0, (Fix64)10), new((Fix64)3, (Fix64)5)));
        bool overflow = false;
        Assert.Equal(-2, MathFix.AddOverflowHelper(-1, -1, ref overflow));
        Assert.False(overflow);
        MathFix.AddOverflowHelper(long.MaxValue, 1, ref overflow);
        Assert.True(overflow);
    }
}
