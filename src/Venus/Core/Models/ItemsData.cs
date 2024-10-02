using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// The data that is displayed by a <see cref="CollectionView{T}"/> component.
/// </summary>
public class ItemsData<T>
{
    /// <summary>
    /// The items that are displayed by the <see cref="CollectionView{T}"/>.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }
}

/// <summary>
/// Data that is displayed when offset pagination is used.
/// </summary>
public class OffsetItemsData<T> : ItemsData<T>
{
    /// <summary>
    /// The total count of the items that are available, including those that are not displayed.
    /// </summary>
    public required int TotalCount { get; init; }
}

/// <summary>
/// Data that is displayed when cursor pagination is used.
/// </summary>
public class CursorItemsData<T> : ItemsData<T>
{
    /// <summary>
    /// The cursor that can be used to retrieve the next page of items.
    /// </summary>
    public required object? NextCursor { get; init; }

    /// <summary>
    /// The cursor that can be used to retrieve the previous page of items.
    /// </summary>
    public required object? PreviousCursor { get; init; }
}
