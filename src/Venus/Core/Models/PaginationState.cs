using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// The pagination state of an <see cref="CollectionView{T}"/> component.
/// </summary>
/// <param name="PageSize">The maximum number of items that can be displayed by the control at once</param>
/// <param name="Sort">The selected sorting option if provided</param>
/// <param name="Filter">The selected filtering option if provided</param>
/// <param name="Ordering">The ordering of the selected sort option</param>
public abstract record PaginationState(int PageSize, object? Sort, object? Filter, Ordering Ordering);

/// <summary>
/// The pagination state of an <see cref="CollectionView{T}"/> component when cursor pagination is used.
/// </summary>
/// <param name="Cursor">The currently active cursor</param>
/// <param name="PageSize">The maximum number of items that can be displayed by the control at once</param>
/// <param name="Sort">The selected sorting option if provided</param>
/// <param name="Filter">The selected filtering option if provided</param>
/// <param name="Ordering">The ordering of the selected sort option</param>
public record CursorPaginationState(object? Cursor, int PageSize, object? Sort, object? Filter, Ordering Ordering)
    : PaginationState(PageSize, Sort, Filter, Ordering);

/// <summary>
/// The pagination state of an <see cref="CollectionView{T}"/> component when offset pagination is used.
/// </summary>
/// <param name="Page">The zero based page that is currently being displayed</param>
/// <param name="PageSize">The maximum number of items that can be displayed by the control at once</param>
/// <param name="Sort">The selected sorting option if provided</param>
/// <param name="Filter">The selected filtering option if provided</param>
/// <param name="Ordering">The ordering of the selected sort option</param>
public record OffsetPaginationState(int Page, int PageSize, object? Sort, object? Filter, Ordering Ordering)
    : PaginationState(PageSize, Sort, Filter, Ordering);
