namespace Ocluse.LiquidSnow.Venus.Blazor.Models
{
    public class ListViewData<T>
    {
        public required IEnumerable<T> Items { get; set; }
    }

    public class OffsetListViewData<T> : ListViewData<T>
    {
        public required int TotalItems { get; set; }
    }

    public class CursorListViewData<T> : ListViewData<T>
    {
        public required object? NextCursor { get; set; }
        public required object? PreviousCursor { get; set; }
    }

    public class PaginationState
    {
        public int PageSize { get; protected set; }

        public object? Sort { get; protected set; }
        
        public object? Filter { get; protected set; }

        public Ordering Ordering { get; protected set; }
    }
    public class CursorPaginationState : PaginationState
    {
        public object? Cursor { get; protected set; }

        public CursorPaginationState Copy()
        {
            return new CursorPaginationState
            {
                PageSize = PageSize,
                Cursor = Cursor
            };
        }

        public CursorPaginationState WithPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }

        public CursorPaginationState WithCursor(object? cursor)
        {
            Cursor = cursor;
            return this;
        }

        public CursorPaginationState WithSort(object? sort)
        {
            Sort = sort;
            return this;
        }

        public CursorPaginationState WithFilter(object? filter)
        {
            Filter = filter;
            return this;
        }

        public CursorPaginationState WithOrdering(Ordering ordering)
        {
            Ordering = ordering;
            return this;
        }
    }
    
    public class OffsetPaginationState : PaginationState
    {
        
        
        public int Page { get; protected set; }
        

        public OffsetPaginationState Copy()
        {
            return new OffsetPaginationState
            {
                Ordering = Ordering,
                PageSize = PageSize,
                Page = Page,
                Sort = Sort,
                Filter = Filter
            };
        }

        public OffsetPaginationState WithPageSize(int size)
        {
            PageSize = size;
            return this;
        }

        public OffsetPaginationState WithPage(int page)
        {
            Page = page;
            return this;
        }

        public OffsetPaginationState WithSort(object? sort)
        {
            Sort = sort;
            return this;
        }

        public OffsetPaginationState WithFilter(object? filter)
        {
            Filter = filter;
            return this;
        }

        public OffsetPaginationState WithOrdering(Ordering ordering)
        {
            Ordering = ordering;
            return this;
        }
    }
}
