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

    /// <summary>
    /// Creates a new, empty instance of the <see cref="LoadResult{TKey, TItem}"/> class with no items and default key values.
    /// </summary>
    /// <remarks>
    /// Use this method to represent a result with no data, such as when a data source returns no
    /// items for a given query.
    /// </remarks>
    /// <returns>An empty <see cref="LoadResult{TKey, TItem}"/> instance with the Items collection empty and both NextKey and PreviousKey set
    /// to their default values.
    /// </returns>
    public static LoadResult<TKey, TItem> Empty()
    {
        return new LoadResult<TKey, TItem>
        {
            NextKey = default,
            PreviousKey = default,
            Items = []
        };
    }
}
