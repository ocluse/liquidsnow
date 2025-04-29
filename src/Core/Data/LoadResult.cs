namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the result of a data load operation.
/// </summary>
/// <typeparam name="TKey">The type of key used to paginate data.</typeparam>
/// <typeparam name="TItem">The type of data.</typeparam>
public record LoadResult<TKey, TItem>
{
    /// <summary>
    /// The key that can be used to load the next page of data.
    /// </summary>
    public TKey? NextKey { get; init; }

    /// <summary>
    /// The key that can be used to load the previous page of data.
    /// </summary>
    public TKey? PreviousKey { get; init; }

    /// <summary>
    /// The data items loaded for the current page.
    /// </summary>
    public required IReadOnlyList<TItem> Items { get; init; }
}
