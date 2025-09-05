namespace Ocluse.LiquidSnow.Numerics.Bounds;

/// <summary>
/// Represents a generic interface for a bounding volume in 3D/2D space with fixed-point precision.
/// </summary>
public interface IFixBound
{
    /// <summary>
    /// The minimum bounds of the IBound.
    /// </summary>
    FixVector3 Min { get; }

    /// <summary>
    /// The maximum bounds of the IBound.
    /// </summary>
    FixVector3 Max { get; }

    /// <summary>
    /// Checks if a point is inside the IBound.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns>True if the point is inside the IBound, otherwise false.</returns>
    bool Contains(FixVector3 point);

    /// <summary>
    /// Checks if the IBound intersects with another IBound.
    /// </summary>
    /// <param name="other">The other IBound to check for intersection.</param>
    /// <returns>True if the IBounds intersect, otherwise false.</returns>
    bool Intersects(IFixBound other);

    /// <summary>
    /// Projects a point onto the IBound. If the point is outside the IBound, it returns the closest point on the surface.
    /// </summary>
    FixVector3 ProjectPoint(FixVector3 point);
}