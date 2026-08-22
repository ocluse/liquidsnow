namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the result of a data load operation.
/// </summary>
/// <typeparam name="TCursor">The type of cursor used to paginate data.</typeparam>
/// <typeparam name="TItem">The type of data.</typeparam>
public record LoadResult<TCursor, TItem>
{
    /// <summary>
    /// The cursor that can be used to load the next page of data.
    /// </summary>
    public TCursor? NextCursor { get; init; }

    /// <summary>
    /// The cursor that can be used to load the previous page of data.
    /// </summary>
    public TCursor? PreviousCursor { get; init; }

    /// <summary>
    /// The data items loaded for the current page.
    /// </summary>
    public required IReadOnlyList<TItem> Items { get; init; }

    /// <summary>
    /// Creates a new, empty instance of the <see cref="LoadResult{TCursor, TItem}"/> class with no items and default cursor values.
    /// </summary>
    /// <remarks>
    /// Use this method to represent a result with no data, such as when a data source returns no
    /// items for a given query.
    /// </remarks>
    /// <returns>An empty <see cref="LoadResult{TCursor, TItem}"/> instance with the Items collection empty and both cursors set
    /// to their default values.
    /// </returns>
    public static LoadResult<TCursor, TItem> Empty()
    {
        return new LoadResult<TCursor, TItem>
        {
            NextCursor = default,
            PreviousCursor = default,
            Items = []
        };
    }
}
