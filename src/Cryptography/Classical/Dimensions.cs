namespace Ocluse.LiquidSnow.Cryptography.Classical;

/// <summary>
/// Represents 2D dimensional info with X and Y
/// </summary>
public struct Dimensions : IEquatable<Dimensions> 
{
    /// <summary>
    /// The number of items in the X (Horizontal) direction.
    /// </summary>
    public int X { get; set; }
    /// <summary>
    /// The number of items in the Y (Vertical) direction
    /// </summary>
    public int Y { get; set; }
    
    /// <summary>
    /// Creates a new instance using the specified dimensions
    /// </summary>
    public Dimensions(int x, int y) { X = x; Y = y; }

    /// <summary>
    /// The product of the <see cref="X"/> and <see cref="Y"/> dimensions
    /// </summary>
    public readonly int Size { get { return X * Y; } }

    /// <summary>
    /// Truncates the dimensions to match the dimensions provided
    /// </summary>
    /// <param name="dimensions">The dimensions to be matched</param>
    public void Limit(Dimensions dimensions)
    {
        Limit(dimensions.X, dimensions.Y);
    }
    /// <summary>
    /// Truncates the dimensions to match the provided X and Y
    /// </summary>
    /// <param name="max_x">The maximum dimension in the X direction</param>
    /// <param name="max_y">The maximum dimension in the Y direction</param>
    public void Limit(int max_x, int max_y)
    {
        X %= max_x;
        Y %= max_y;
    }


    #region Object Overrides
    ///<inheritdoc/>
    public readonly bool Equals(Dimensions other)
    {
        return X == other.X && Y == other.Y;
    }
    ///<inheritdoc/>
    public override readonly bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(Dimensions)) return false;
        var other = (Dimensions)obj;

        return X == other.X && Y == other.Y;
    }
    ///<inheritdoc/>
    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }
    /// <summary>
    /// Checks if two dimensions are equal
    /// </summary>
    public static bool operator == (Dimensions d1, Dimensions d2)
    {
        return d1.Equals(d2);
    }
    /// <summary>
    /// Checks if two dimensions are not equal
    /// </summary>
    public static bool operator !=(Dimensions d1, Dimensions d2)
    {
        return !d1.Equals(d2);
    }

    /// <summary>
    /// Returns a neatly formatted string of the dimensions, e.g. 2x4
    /// </summary>
    public override readonly string ToString()
    {
        return $"{X}x{Y}";
    }

    #endregion
}
