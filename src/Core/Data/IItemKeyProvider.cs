namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Exposes the stable key used to identify an item.
/// </summary>
/// <typeparam name="TItem">The type of item being identified.</typeparam>
public interface IItemKeyProvider<in TItem>
{
    /// <summary>
    /// Gets the stable key for an item.
    /// </summary>
    object GetItemKey(TItem item);
}
