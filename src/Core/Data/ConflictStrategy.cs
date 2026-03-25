namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines how to resolve add conflicts for keyed mutable pagers.
/// </summary>
public enum ConflictStrategy
{
    /// <summary>
    /// Replace the existing item with the new item.
    /// </summary>
    Replace,

    /// <summary>
    /// Ignore the new item and keep the existing item.
    /// </summary>
    Ignore,

    /// <summary>
    /// Throw an exception if an item with the same key already exists.
    /// </summary>
    Error
}
