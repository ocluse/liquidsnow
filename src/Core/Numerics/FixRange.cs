#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;
/// <summary>
/// Represents a range of values with fixed precision.
/// </summary>
[Serializable]
public struct FixRange : IEquatable<FixRange>
{
    #region Constants

    /// <summary>
    /// The smallest possible range.
    /// </summary>
    public static readonly FixRange MinRange = new(Fix64.MIN_VALUE, Fix64.MIN_VALUE);

    /// <summary>
    /// The largest possible range.
    /// </summary>
    public static readonly FixRange MaxRange = new(Fix64.MAX_VALUE, Fix64.MAX_VALUE);

    #endregion

    #region Fields

    /// <summary>
    /// Gets the minimum value of the range.
    /// </summary>
    [JsonInclude]
    public Fix64 Min;

    /// <summary>
    /// Gets the maximum value of the range.
    /// </summary>
    [JsonInclude] 
    public Fix64 Max;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the FixedRange structure with the specified minimum and maximum values.
    /// </summary>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <param name="enforceOrder">If true, ensures that Min is less than or equal to Max.</param>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixRange(Fix64 min, Fix64 max, bool enforceOrder = true)
    {
        if (enforceOrder)
        {
            Min = min < max ? min : max;
            Max = min < max ? max : min;
        }
        else
        {
            Min = min;
            Max = max;
        }
    }

    #endregion

    #region Properties and Methods (Instance)

    /// <summary>
    /// The length of the range, computed as Max - Min.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Max - Min;
    }

    /// <summary>
    /// The midpoint of the range.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 MidPoint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (Min + Max) * Fix64.Half;
    }

    /// <summary>
    /// Sets the minimum and maximum values for the range.
    /// </summary>
    /// <param name="min">The new minimum value.</param>
    /// <param name="max">The new maximum value.</param>
    public void SetMinMax(Fix64 min, Fix64 max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Adds a value to both the minimum and maximum of the range.
    /// </summary>
    /// <param name="val">The value to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInPlace(Fix64 val)
    {
        Min += val;
        Max += val;
    }

    /// <summary>
    /// Determines whether the specified value is within the range, with an option to include or exclude the upper bound.
    /// </summary>
    /// <param name="x">The value to check.</param>
    /// <param name="includeMax">If true, the upper bound (Max) is included in the range check; otherwise, the upper bound is exclusive. Default is false (exclusive).</param>
    /// <returns>True if the value is within the range; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool InRange(Fix64 x, bool includeMax = false)
    {
        return includeMax ? x >= Min && x <= Max : x >= Min && x < Max;
    }

    /// <inheritdoc cref="InRange(Fix64, bool)" />
    public readonly bool InRange(double x, bool includeMax = false)
    {
        long xL = (long)Math.Round((double)x * MathFix.ONE_L);
        return includeMax ? xL >= Min.RawValue && xL <= Max.RawValue : xL >= Min.RawValue && xL < Max.RawValue;
    }

    /// <summary>
    /// Checks whether this range overlaps with the specified range, ensuring no adjacent edges are considered overlaps.
    /// </summary>
    /// <param name="other">The range to compare.</param>
    /// <returns>True if the ranges overlap; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Overlaps(FixRange other)
    {
        return Min < other.Max && Max > other.Min;
    }

    #endregion

    #region Range Operations

    /// <summary>
    /// Determines the direction from one range to another.
    /// If they don't overlap, returns -1 or 1 depending on the relative position.
    /// </summary>
    /// <param name="range1">The first range.</param>
    /// <param name="range2">The second range.</param>
    /// <param name="sign">The direction between ranges (-1 or 1).</param>
    /// <returns>True if the ranges don't overlap, false if they do.</returns>
    public static bool GetDirection(FixRange range1, FixRange range2, out Fix64? sign)
    {
        sign = null;
        if (!range1.Overlaps(range2))
        {
            if (range1.Max < range2.Min) sign = -Fix64.One;
            else sign = Fix64.One;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates the overlap depth between two ranges.
    /// Assumes the ranges are sorted (min and max are correctly assigned).
    /// </summary>
    /// <param name="rangeA">The first range.</param>
    /// <param name="rangeB">The second range.</param>
    /// <returns>The depth of the overlap between the ranges.</returns>
    public static Fix64 ComputeOverlapDepth(FixRange rangeA, FixRange rangeB)
    {
        // Check if one range is completely within the other
        bool isRangeAInsideB = rangeA.Min >= rangeB.Min && rangeA.Max <= rangeB.Max;
        bool isRangeBInsideA = rangeB.Min >= rangeA.Min && rangeB.Max <= rangeA.Max;
        if (isRangeAInsideB)
            return rangeA.Max - rangeB.Min; // The size of rangeA
        else if (isRangeBInsideA)
            return rangeB.Max - rangeA.Min; // The size of rangeB

        // Calculate overlap between the two ranges
        Fix64 overlapEnd = MathFix.Min(rangeA.Max, rangeB.Max);
        Fix64 overlapStart = MathFix.Max(rangeA.Min, rangeB.Min);
        Fix64 overlap = overlapEnd - overlapStart;

        return overlap > Fix64.Zero ? overlap : Fix64.Zero;
    }

    /// <summary>
    /// Checks for overlap between two ranges and calculates the vector of overlap depth.
    /// </summary>
    /// <param name="origin">The origin vector.</param>
    /// <param name="range1">The first range.</param>
    /// <param name="range2">The second range.</param>
    /// <param name="limit">The overlap limit to check.</param>
    /// <param name="sign">The direction sign to consider.</param>
    /// <param name="output">The overlap vector and depth, if any.</param>
    /// <returns>True if overlap occurs and is below the limit, otherwise false.</returns>
    public static bool CheckOverlap(FixVector3 origin, FixRange range1, FixRange range2, Fix64 limit, Fix64 sign, out (FixVector3 Vector, Fix64 Depth)? output)
    {
        output = null;
        Fix64 overlap = ComputeOverlapDepth(range1, range2);

        // If the overlap is smaller than the current minimum, update the minimum
        if (overlap < limit)
        {
            output = (origin * overlap * sign, overlap);
            return true;
        }
        return false;
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixRange operator +(FixRange left, FixRange right)
    {
        return new FixRange(left.Min + right.Min, left.Max + right.Max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixRange operator -(FixRange left, FixRange right)
    {
        return new FixRange(left.Min - right.Min, left.Max - right.Max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixRange left, FixRange right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixRange left, FixRange right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Conversion

    /// <summary>
    /// Returns a string that represents the FixedRange instance, formatted as "Min - Max".
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return $"{Min.ToFormattedDouble()} - {Max.ToFormattedDouble()}";
    }

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is FixRange other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixRange other)
    {
        return other.Min == Min && other.Max == Max;
    }

    /// <summary>
    /// Computes the hash code for the FixedRange instance.
    /// </summary>
    /// <returns>The hash code of the range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return Min.GetHashCode() ^ Max.GetHashCode();
    }

    #endregion
}
