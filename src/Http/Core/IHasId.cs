namespace Ocluse.LiquidSnow.Http;

/// <summary>
/// Represents an object that has an ID.
/// </summary>
public interface IHasId<TId>
{
    /// <summary>
    /// Gets the ID of the object.
    /// </summary>
    TId Id { get; }
}
