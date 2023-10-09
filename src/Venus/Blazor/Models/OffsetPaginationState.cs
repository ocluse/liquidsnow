namespace Ocluse.LiquidSnow.Venus.Blazor.Models
{
    public class ListViewData<T>
    {
        public required IEnumerable<T> Items { get; set; }
    }

    public class OffsetListViewData<T> : ListViewData<T>
    {
        public required int TotalCount { get; set; }
    }

    public class CursorListViewData<T> : ListViewData<T>
    {
        public required object? NextCursor { get; set; }
        public required object? PreviousCursor { get; set; }
    }

    public record PaginationState(int PageSize, object? Sort, object? Filter, Ordering Ordering);

    public record CursorPaginationState(object? Cursor, int PageSize, object? Sort, object? Filter, Ordering Ordering) 
        : PaginationState(PageSize, Sort, Filter, Ordering);
    
    public record OffsetPaginationState(int Page, int PageSize, object? Sort, object? Filter, Ordering Ordering)
        : PaginationState(PageSize, Sort, Filter, Ordering);
}
