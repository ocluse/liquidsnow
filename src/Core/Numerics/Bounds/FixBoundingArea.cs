#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Bounds;
/// <summary>
/// Represents a lightweight, axis-aligned bounding area with fixed-point precision, optimized for 2D or simplified 3D use cases.
/// </summary>
/// <remarks>
/// The BoundingArea is designed for performance-critical scenarios where only a minimal bounding volume is required.
/// It offers fast containment and intersection checks with other bounds but lacks the full feature set of BoundingBox.
/// 
/// Use Cases:
/// - Efficient spatial queries in 2D or constrained 3D spaces (e.g., terrain maps or collision grids).
/// - Simplified bounding volume checks where rotation or complex shape fitting is not needed.
/// - Can be used as a broad-phase bounding volume to cull objects before more precise checks with BoundingBox or BoundingSphere.
/// </remarks>

[Serializable]
public struct FixBoundingArea : IFixBound, IEquatable<FixBoundingArea>
{
    #region Fields

    /// <summary>
    /// One of the corner points of the bounding area.
    /// </summary>
    [JsonInclude]
    public FixVector3 Corner1;

    /// <summary>
    /// The opposite corner point of the bounding area.
    /// </summary>
    [JsonInclude]
    public FixVector3 Corner2;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the BoundingArea struct with corner coordinates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixBoundingArea(Fix64 c1x, Fix64 c1y, Fix64 c1z, Fix64 c2x, Fix64 c2y, Fix64 c2z)
    {
        Corner1 = new FixVector3(c1x, c1y, c1z);
        Corner2 = new FixVector3(c2x, c2y, c2z);
    }

    /// <summary>
    /// Initializes a new instance of the BoundingArea struct with two corner points.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixBoundingArea(FixVector3 corner1, FixVector3 corner2)
    {
        Corner1 = corner1;
        Corner2 = corner2;
    }

    #endregion

    #region Properties and Methods (Instance)

    // Min/Max properties for easy access to boundaries

    /// <summary>
    /// The minimum corner of the bounding box.
    /// </summary>
    [JsonIgnore]
    public readonly FixVector3 Min
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(MinX, MinY, MinZ);
    }

    /// <summary>
    /// The maximum corner of the bounding box.
    /// </summary>
    [JsonIgnore]
    public readonly FixVector3 Max
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(MaxX, MaxY, MaxZ);
    }

    [JsonIgnore]
    public readonly Fix64 MinX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.X < Corner2.X ? Corner1.X : Corner2.X;
    }

    [JsonIgnore]
    public readonly Fix64 MaxX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.X > Corner2.X ? Corner1.X : Corner2.X;
    }

    [JsonIgnore]
    public readonly Fix64 MinY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.Y < Corner2.Y ? Corner1.Y : Corner2.Y;
    }

    [JsonIgnore]
    public readonly Fix64 MaxY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.Y > Corner2.Y ? Corner1.Y : Corner2.Y;
    }

    [JsonIgnore]
    public readonly Fix64 MinZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.Z < Corner2.Z ? Corner1.Z : Corner2.Z;
    }

    [JsonIgnore]
    public readonly Fix64 MaxZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Corner1.Z > Corner2.Z ? Corner1.Z : Corner2.Z;
    }

    /// <summary>
    /// Calculates the width (X-axis) of the bounding area.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MaxX - MinX;
    }

    /// <summary>
    /// Calculates the height (Y-axis) of the bounding area.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 Height
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MaxY - MinY;
    }

    /// <summary>
    /// Calculates the depth (Z-axis) of the bounding area.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 Depth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MaxZ - MinZ;
    }

    /// <summary>
    /// Determines if a point is inside the bounding area (including boundaries).
    /// </summary>
    public readonly bool Contains(FixVector3 point)
    {
        // Check if the point is within the bounds of the area (including boundaries)
        return point.X >= MinX && point.X <= MaxX
            && point.Y >= MinY && point.Y <= MaxY
            && point.Z >= MinZ && point.Z <= MaxZ;
    }

    /// <summary>
    /// Checks if another IBound intersects with this bounding area.
    /// </summary>
    /// <remarks>
    /// It checks for overlap on all axes. If there is no overlap on any axis, they do not intersect.
    /// </remarks>
    public readonly bool Intersects(IFixBound other)
    {
        switch (other)
        {
            case FixBoundingBox or FixBoundingArea:
                {
                    if (Contains(other.Min) && Contains(other.Max))
                        return true;  // Full containment

                    // General intersection logic (allowing for overlap)
                    return !(Max.X <= other.Min.X || Min.X >= other.Max.X ||
                             Max.Y <= other.Min.Y || Min.Y >= other.Max.Y ||
                             Max.Z <= other.Min.Z || Min.Z >= other.Max.Z);
                }
            case FixBoundingSphere sphere:
                // Find the closest point on the area to the sphere's center
                // Intersection occurs if the distance from the closest point to the sphere’s center is within the radius.
                return FixVector3.SqrDistance(sphere.Center, this.ProjectPointWithinBounds(sphere.Center)) <= sphere.SqrRadius;

            default: return false; // Default case for unknown or unsupported types
        }
        ;
    }

    /// <summary>
    /// Projects a point onto the bounding box. If the point is outside the box, it returns the closest point on the surface.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector3 ProjectPoint(FixVector3 point)
    {
        return this.ProjectPointWithinBounds(point);
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixBoundingArea left, FixBoundingArea right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixBoundingArea left, FixBoundingArea right) => !left.Equals(right);

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj) => obj is FixBoundingArea other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixBoundingArea other) => Corner1.Equals(other.Corner1) && Corner2.Equals(other.Corner2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Corner1.GetHashCode();
            hash = hash * 23 + Corner2.GetHashCode();
            return hash;
        }
    }

    #endregion
}