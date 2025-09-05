#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Bounds;
/// <summary>
/// Represents an axis-aligned bounding box with fixed-point precision, capable of encapsulating 3D objects for more complex spatial checks.
/// </summary>
[Serializable]
public struct FixBoundingBox : IFixBound, IEquatable<FixBoundingBox>, IDeserializationCallback
{
    #region Fields

    /// <summary>
    /// The center of the bounding box.
    /// </summary>
    private FixVector3 _center;

    private FixVector3 _size;

    /// <summary>
    /// The range (half-size) of the bounding box in all directions. Always half of the total size.
    /// </summary>
    private FixVector3 _scope;

    /// <summary>
    /// The minimum corner of the bounding box.
    /// </summary>
    private FixVector3 _min;

    /// <summary>
    /// The maximum corner of the bounding box.
    /// </summary>
    private FixVector3 _max;

    private bool _isDirty;

    /// <summary>
    /// Vertices of the bounding box.
    /// </summary>
    private FixVector3[] _vertices;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the BoundingBox struct with the specified center and size.
    /// </summary>

    public FixBoundingBox(FixVector3 center, FixVector3 size)
    {
        _vertices = new FixVector3[8];

        _center = center;
        _size = size;
        _scope = default;
        _min = default;
        _max = default;
        _isDirty = true;

        Recalculate();
    }

    #endregion

    #region Properties and Methods (Instance)

    /// <inheritdoc cref="_center" />
    public FixVector3 Center
    {
        readonly get => _center;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _center = value;
            Recalculate();
        }
    }

    /// <inheritdoc cref="_scope" />
    [JsonIgnore]
    public readonly FixVector3 Scope
    {
        get => _scope;
    }

    /// <inheritdoc cref="_min" />
    [JsonIgnore]
    public readonly FixVector3 Min
    {
        get => _min;
    }

    /// <inheritdoc cref="_max" />
    [JsonIgnore]
    public readonly FixVector3 Max
    {
        get => _max;
    }

    /// <inheritdoc cref="_vertices" />
    [JsonIgnore]
    public FixVector3[] Vertices
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GenerateVertices();
    }

    /// <summary>
    /// The total size of the box (Width, Height, Depth). This is always twice the scope.
    /// </summary>
    public FixVector3 Proportions
    {
        readonly get => _size;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _size = value;
            Recalculate();
        }
    }

    /// <summary>
    /// Orients the bounding box with the given center and size.
    /// </summary>
    public void Orient(FixVector3 center, FixVector3? size)
    {
        FixVector3 scope = size.HasValue ? size.Value * Fix64.Half : Scope;
        SetBoundingBox(center, scope);
    }

    /// <summary>
    /// Resizes the bounding box to the specified size, keeping the same center.
    /// </summary>
    public void Resize(FixVector3 size)
    {
        SetBoundingBox(_center, size * Fix64.Half);
    }

    /// <summary>
    /// Sets the bounds of the bounding box by specifying its minimum and maximum points.
    /// </summary>
    public void SetMinMax(FixVector3 min, FixVector3 max)
    {
        FixVector3 newScope = (max - min) * Fix64.Half;
        SetBoundingBox(min + newScope, newScope);
    }

    /// <summary>
    /// Configures the bounding box with the specified center and scope (half-size).
    /// </summary>
    public void SetBoundingBox(FixVector3 center, FixVector3 scope)
    {
        this._center = center;
        _scope = scope;
        _min = this._center - Scope;
        _max = this._center + Scope;
        _isDirty = true;
    }

    /// <summary>
    /// Determines if a point is inside the bounding box (including boundaries).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(FixVector3 point)
    {
        return point.X >= Min.X && point.X <= Max.X
            && point.Y >= Min.Y && point.Y <= Max.Y
            && point.Z >= Min.Z && point.Z <= Max.Z;
    }

    /// <summary>
    /// Checks if another IBound intersects with this bounding box.
    /// Ensures overlap, not just touching boundaries.
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
                // project the sphere’s center onto the 3D volume and checks the distance to the surface.
                // If the distance from the closest point to the center is less than or equal to the sphere’s radius, they intersect.
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

    /// <summary>
    /// Calculates the shortest distance from a given point to the surface of the bounding box.
    /// If the point lies inside the box, the distance is zero.
    /// </summary>
    /// <param name="point">The point from which to calculate the distance.</param>
    /// <returns>
    /// The shortest distance from the point to the surface of the bounding box.
    /// If the point is inside the box, the method returns zero.
    /// </returns>
    /// <remarks>
    /// The method finds the closest point on the box's surface by clamping the given point 
    /// to the box's bounds and returns the Euclidean distance between them. 
    /// This ensures accurate distance calculations, even near corners or edges.
    /// </remarks>
    public readonly Fix64 DistanceToSurface(FixVector3 point)
    {
        // Clamp the point to the nearest point on the box's surface
        FixVector3 clampedPoint = new(
            MathFix.Clamp(point.X, Min.X, Max.X),
            MathFix.Clamp(point.Y, Min.Y, Max.Y),
            MathFix.Clamp(point.Z, Min.Z, Max.Z)
        );

        // If the point is inside the box, return 0
        if (Contains(point))
        {
            return Fix64.Zero;
        }

        // Otherwise, return the Euclidean distance to the clamped point
        return FixVector3.Distance(point, clampedPoint);
    }

    /// <summary>
    /// Finds the closest point on the surface of the bounding box towards a specified object position.
    /// </summary>
    public readonly FixVector3 GetPointOnSurfaceTowardsObject(FixVector3 objectPosition)
    {
        return ClosestPointOnSurface(ProjectPoint(objectPosition));
    }

    /// <summary>
    /// Finds the closest point on the surface of the bounding box to the specified point.
    /// </summary>
    public readonly FixVector3 ClosestPointOnSurface(FixVector3 point)
    {
        if (Contains(point))
        {
            // Calculate distances to each face and return the closest face.
            Fix64 distToMinX = point.X - Min.X;
            Fix64 distToMaxX = Max.X - point.X;
            Fix64 distToMinY = point.Y - Min.Y;
            Fix64 distToMaxY = Max.Y - point.Y;
            Fix64 distToMinZ = point.Z - Min.Z;
            Fix64 distToMaxZ = Max.Z - point.Z;

            Fix64 minDistToFace = MathFix.Min(distToMinX, MathFix.Min(distToMaxX, MathFix.Min(distToMinY, MathFix.Min(distToMaxY, MathFix.Min(distToMinZ, distToMaxZ)))));

            // Adjust the closest point based on the face.
            if (minDistToFace == distToMinX) point.X = Min.X;
            else if (minDistToFace == distToMaxX) point.X = Max.X;

            if (minDistToFace == distToMinY) point.Y = Min.Y;
            else if (minDistToFace == distToMaxY) point.Y = Max.Y;

            if (minDistToFace == distToMinZ) point.Z = Min.Z;
            else if (minDistToFace == distToMaxZ) point.Z = Max.Z;

            return point;
        }

        // If the point is outside the box, clamp to the nearest surface.
        return new FixVector3(
            MathFix.Clamp(point.X, Min.X, Max.X),
            MathFix.Clamp(point.Y, Min.Y, Max.Y),
            MathFix.Clamp(point.Z, Min.Z, Max.Z)
        );
    }

    /// <summary>
    /// Generates the vertices of the bounding box based on its center and scope.
    /// </summary>
    /// <remarks>
    ///  Vertices[0]  near Bot left 
    ///  Vertices[1]  near Bot right 
    ///  Vertices[2]  near Top left
    ///  Vertices[3]  near Top right
    ///  Vertices[4]  far bot left
    ///  Vertices[5]  far bot right
    ///  Vertices[6]  far top left
    ///  Vertices[7]  far top right
    ///  ----
    ///  near quad
    ///  0 - 1 Bot left near to bot right near
    ///  2 - 3 Top left near to top right near
    ///  0 - 2 Bot left near to top left near
    ///  1 - 3 Bot right near to top right near
    ///  far quad
    ///  4 - 5 Bot left far to bot right far
    ///  6 - 7 Top left far to top right far
    ///  4 - 6 Bot left far to top left far
    ///  5 - 7 Bot right far to top right far
    ///  lines connecting near and far quads
    ///  0 - 4 Bot left near to bot left far
    ///  1 - 5 Bot right near to bot right far
    ///  2 - 6 Top left near to top left far
    ///  3 - 7 Top right near to top right far  
    /// </remarks>
    private FixVector3[] GenerateVertices()
    {
        if (_isDirty)
        {
            _vertices[0] = _center + _vertices[0].Set(-Scope.X, -Scope.Y, -Scope.Z);
            _vertices[1] = _center + _vertices[1].Set(Scope.X, -Scope.Y, -Scope.Z);
            _vertices[2] = _center + _vertices[2].Set(-Scope.X, Scope.Y, -Scope.Z);
            _vertices[3] = _center + _vertices[3].Set(Scope.X, Scope.Y, -Scope.Z);
            _vertices[4] = _center + _vertices[4].Set(-Scope.X, -Scope.Y, Scope.Z);
            _vertices[5] = _center + _vertices[5].Set(Scope.X, -Scope.Y, Scope.Z);
            _vertices[6] = _center + _vertices[6].Set(-Scope.X, Scope.Y, Scope.Z);
            _vertices[7] = _center + _vertices[7].Set(Scope.X, Scope.Y, Scope.Z);

            _isDirty = false;
        }

        return _vertices;
    }

    #endregion

    #region BoundingBox Operations

    /// <summary>
    /// Creates a new bounding box that is the union of two bounding boxes.
    /// </summary>
    public static FixBoundingBox Union(FixBoundingBox a, FixBoundingBox b)
    {
        FixVector3 min = FixVector3.Min(a.Min, b.Min);
        FixVector3 max = FixVector3.Max(a.Max, b.Max);

        FixVector3 center = (max + min) * Fix64.Half;
        FixVector3 size = max - min;

        return new FixBoundingBox(center, size);
    }

    /// <summary>
    /// Finds the closest points between two bounding boxes.
    /// </summary>
    public static FixVector3 FindClosestPointsBetweenBoxes(FixBoundingBox a, FixBoundingBox b)
    {
        FixVector3 closestPoint = FixVector3.Zero;
        Fix64 minDistance = Fix64.MAX_VALUE;
        for (int i = 0; i < b.Vertices.Length; i++)
        {
            FixVector3 point = a.ClosestPointOnSurface(b.Vertices[i]);
            Fix64 distance = FixVector3.Distance(point, b.Vertices[i]);
            if (distance < minDistance)
            {
                closestPoint = point;
                minDistance = distance;
            }
        }

        return closestPoint;
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixBoundingBox left, FixBoundingBox right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixBoundingBox left, FixBoundingBox right) => !left.Equals(right);

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj) => obj is FixBoundingBox other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixBoundingBox other) => _center.Equals(other._center) && Scope.Equals(other.Scope);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + _center.GetHashCode();
            hash = hash * 23 + Scope.GetHashCode();
            return hash;
        }
    }

    #endregion

    #region Recalc helper

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Recalculate()
    {
        _vertices ??= new FixVector3[8];

        // half-extent
        _scope = _size * Fix64.Half;
        _min = _center - _scope;
        _max = _center + _scope;
        _isDirty = true;
    }

    #endregion

    #region Serialization callbacks

    // BinaryFormatter (and many serializers) will call this after fields are restored:
    void IDeserializationCallback.OnDeserialization(object? sender)
        => Recalculate();

    [OnDeserialized]
    private void OnJsonDeserialized(StreamingContext _)
        => Recalculate();


    #endregion
}