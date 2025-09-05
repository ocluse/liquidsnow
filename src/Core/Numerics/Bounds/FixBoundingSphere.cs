#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics.Bounds;
/// <summary>
/// Represents a spherical bounding volume with fixed-point precision, optimized for fast, rotationally invariant spatial checks in 3D space.
/// </summary>
/// <remarks>
/// Initializes a new instance of the BoundingSphere struct with the specified center and radius.
/// </remarks>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct FixBoundingSphere(FixVector3 center, Fix64 radius) : IFixBound, IEquatable<FixBoundingSphere>
{
    #region Fields

    /// <summary>
    /// The center point of the sphere.
    /// </summary>
    [JsonInclude]
    public FixVector3 Center = center;

    /// <summary>
    /// The radius of the sphere.
    /// </summary>
    [JsonInclude]
    public Fix64 Radius = radius;

    #endregion
    #region Constructors

    #endregion

    #region Properties and Methods (Instance)

    [JsonIgnore]
    public readonly FixVector3 Min
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Center - new FixVector3(Radius, Radius, Radius);
    }

    [JsonIgnore]
    public readonly FixVector3 Max
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Center + new FixVector3(Radius, Radius, Radius);
    }

    /// <summary>
    /// The squared radius of the sphere.
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 SqrRadius
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Radius * Radius;
    }

    /// <summary>
    /// Checks if a point is inside the sphere.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is inside the sphere, otherwise false.</returns>
    public readonly bool Contains(FixVector3 point)
    {
        return FixVector3.SqrDistance(Center, point) <= SqrRadius;
    }

    /// <summary>
    /// Checks if this sphere intersects with another IBound.
    /// </summary>
    /// <param name="other">The other IBound to check for intersection.</param>
    /// <returns>True if the IBounds intersect, otherwise false.</returns>
    public readonly bool Intersects(IFixBound other)
    {
        switch (other)
        {
            case FixBoundingBox or FixBoundingArea:
                // Find the closest point on the BoundingArea to the sphere's center
                // Check if the closest point is within the sphere's radius
                return FixVector3.SqrDistance(Center, other.ProjectPointWithinBounds(Center)) <= SqrRadius;
            case FixBoundingSphere otherSphere:
                {
                    Fix64 distanceSquared = FixVector3.SqrDistance(Center, otherSphere.Center);
                    Fix64 combinedRadius = Radius + otherSphere.Radius;
                    return distanceSquared <= combinedRadius * combinedRadius;
                }

            default: return false; // Default case for unknown or unsupported types
        }
        ;
    }

    /// <summary>
    /// Projects a point onto the bounding sphere. If the point is outside the sphere, it returns the closest point on the surface.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector3 ProjectPoint(FixVector3 point)
    {
        var direction = point - Center;
        if (direction.IsZero) return Center; // If the point is the center, return the center itself

        return Center + direction.Normalize() * Radius;
    }

    /// <summary>
    /// Calculates the distance from a point to the surface of the sphere.
    /// </summary>
    /// <param name="point">The point to calculate the distance from.</param>
    /// <returns>The distance from the point to the surface of the sphere.</returns>
    public readonly Fix64 DistanceToSurface(FixVector3 point)
    {
        return FixVector3.Distance(Center, point) - Radius;
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixBoundingSphere left, FixBoundingSphere right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixBoundingSphere left, FixBoundingSphere right) => !left.Equals(right);

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj) => obj is FixBoundingSphere other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixBoundingSphere other) => Center.Equals(other.Center) && Radius.Equals(other.Radius);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Center.GetHashCode();
            hash = hash * 23 + Radius.GetHashCode();
            return hash;
        }
    }

    #endregion
}