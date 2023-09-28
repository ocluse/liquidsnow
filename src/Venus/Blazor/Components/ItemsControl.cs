using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemsControl<T> : ContainerBase
    {
        private int _totalItems;

        private object? _nextCursor, _previousCursor;

        private FilterOption? _selectedFilterOption, _selectedSortOption;

        #region Properties

        protected virtual string ItemElement { get; } = "div";

        protected virtual string ContainerElement { get; } = "div";

        #endregion

        #region Parameters
        [Parameter]
        public string? ItemClass { get; set; }

        [Parameter]
        public FiltrationOptions? FiltrationOptions { get; set; }

        [Parameter]
        public RenderFragment? Header { get; set; }

        [Parameter]
        public RenderFragment<T>? ItemTemplate { get; set; }

        [Parameter]
        public IEnumerable<T>? Items { get; set; }

        [Parameter]
        public Func<CursorPaginationState, Task<CursorListViewData<T>>>? CursorFetch { get; set; }

        [Parameter]
        public Func<OffsetPaginationState, Task<OffsetListViewData<T>>>? OffsetFetch { get; set; }

        [Parameter]
        public int? PageSize { get; set; }

        [Parameter]
        public OffsetPaginationState OffsetPaginationState { get; set; } = new();

        [Parameter]
        public EventCallback<OffsetPaginationState> OffsetPaginationStateChanged { get; set; }

        [Parameter]
        public CursorPaginationState CursorPaginationState { get; set; } = new();

        [Parameter]
        public EventCallback<CursorPaginationState> CursorPaginationStateChanged { get; set; }

        [Parameter]
        public string? DisplayMemberPath { get; set; }

        [Parameter]
        public Func<T?, string>? DisplayMemberFunc { get; set; }

        [Parameter]
        public EventCallback<T> ItemClicked { get; set; }
        #endregion

        protected override async Task OnParametersSetAsync()
        {
            if (CursorFetch == null && OffsetFetch == null)
            {
                await ReloadData();
            }
        }

        protected override async Task<int> FetchDataAsync()
        {
            if (CursorFetch != null)
            {
                var result = await CursorFetch.Invoke(CursorPaginationState);
                Items = result.Items;
                _nextCursor = result.NextCursor;
                _previousCursor = result.PreviousCursor;
            }
            else if (OffsetFetch != null)
            {
                var state = OffsetPaginationState.Copy();
                state.WithPageSize(PageSize ?? state.PageSize);
                var result = await OffsetFetch.Invoke(state);
                Items = result.Items;
                _totalItems = result.TotalItems;
            }

            return Items?.Any() == true ? ContainerState.Found : ContainerState.Empty;
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("items-control");
        }

        protected virtual string GetItemClass()
        {
            return $"item-view {ItemClass}";
        }

        private Task FilterChanged(FilterOption? option)
        {
            _selectedFilterOption = option;
            object? filter = option?.Value ?? FiltrationOptions?.DefaultFilter.Value;
            var newCursorState = CursorPaginationState.Copy().WithFilter(filter);
            var newOffsetState = OffsetPaginationState.Copy().WithFilter(filter);
            return SetPaginationState(newOffsetState, newCursorState);
        }

        public Task SorterChanged(FilterOption? option)
        {
            _selectedSortOption = option;

            object? sort = option?.Value ?? FiltrationOptions?.DefaultSort.Value;

            var newCursorState = CursorPaginationState.Copy().WithSort(sort);
            var newOffsetState = OffsetPaginationState.Copy().WithSort(sort);

            if (option is SortOption sortOption)
            {
                newCursorState.WithOrdering(sortOption.Ordering);
                newOffsetState.WithOrdering(sortOption.Ordering);
            }

            return SetPaginationState(newOffsetState, newCursorState);
        }

        private Task OnCursorChanged(object newCursor)
        {
            var newCursorState = CursorPaginationState.Copy().WithCursor(newCursor);
            return SetPaginationState(OffsetPaginationState, newCursorState);
        }

        private Task OnPageChanged(int newPage)
        {
            var newOffsetState = OffsetPaginationState.Copy().WithPage(newPage);
            return SetPaginationState(newOffsetState, CursorPaginationState);
        }

        private async Task SetPaginationState(OffsetPaginationState offsetPagination, CursorPaginationState cursorPagination)
        {
            if (OffsetFetch != null)
            {
                OffsetPaginationState = offsetPagination.Copy();
                await InvokeAsync(async () => await OffsetPaginationStateChanged.InvokeAsync(OffsetPaginationState));
            }

            if (CursorFetch != null)
            {
                CursorPaginationState = cursorPagination.Copy();
                await InvokeAsync(async () => await CursorPaginationStateChanged.InvokeAsync(CursorPaginationState));
            }


            await ReloadData();
        }

        protected override void BuildFound(RenderTreeBuilder builder)
        {
            string itemClass = GetItemClass();

            if (Items != null && Items.Any())
            {
                foreach (var item in Items)
                {
                    builder.OpenElement(52, ItemElement);
                    builder.SetKey(item);
                    builder.AddAttribute(53, "class", itemClass);
                    builder.AddAttribute(54, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
                    if (ItemTemplate == null)
                    {

                        builder.AddContent(55, item.GetDisplayMember(DisplayMemberFunc, DisplayMemberPath));
                    }
                    else
                    {
                        builder.AddContent(59, ItemTemplate, item);
                    }
                    builder.CloseElement();
                }
            }
            else if (EmptyTemplate != null)
            {
                builder.AddContent(60, EmptyTemplate);
            }
            else
            {
                Type typeToRender = Resolver.ResolveContainerStateToRenderType(ContainerState.Empty);
                builder.OpenComponent(61, typeToRender);
                builder.CloseComponent();
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");

            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() },
            };

            builder.AddMultipleAttributes(1, attributes);

            if (Header != null || FiltrationOptions != null)
            {
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "items-header");

                if (Header != null)
                {
                    builder.OpenElement(4, "div");
                    builder.AddAttribute(5, "class", "header-content");
                    builder.AddContent(6, Header);
                    builder.CloseElement();
                }

                if (FiltrationOptions != null)
                {
                    builder.OpenElement(7, "div");
                    builder.AddAttribute(8, "class", "items-filtration");
                    builder.OpenComponent<FilterDropdown>(9);
                    builder.AddAttribute(10, nameof(FilterDropdown.Icon), FeatherIcons.Filter);
                    builder.AddAttribute(11, nameof(FilterDropdown.Value), _selectedFilterOption);
                    builder.AddAttribute(12, nameof(FilterDropdown.Placeholder), "Filter By");
                    builder.AddAttribute(13, nameof(FilterDropdown.ValueChanged), FilterChanged);
                    builder.CloseComponent();

                    builder.OpenComponent<FilterDropdown>(14);
                    builder.AddAttribute(15, nameof(FilterDropdown.Icon), FeatherIcons.List);
                    builder.AddAttribute(16, nameof(FilterDropdown.Value), _selectedSortOption);
                    builder.AddAttribute(17, nameof(FilterDropdown.Placeholder), "Sort By");
                    builder.AddAttribute(18, nameof(FilterDropdown.ValueChanged), SorterChanged);
                    builder.CloseComponent();
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            builder.OpenElement(20, ContainerElement);
            builder.AddAttribute(21, "class", "items-container");
            BuildContainer(builder);
            builder.CloseElement();

            if (CursorFetch != null)
            {
                builder.OpenComponent<PaginationCursor>(520);
                builder.AddAttribute(522, nameof(PaginationCursor.CursorChanged), EventCallback.Factory.Create(this, OnCursorChanged));
                builder.AddAttribute(523, nameof(PaginationCursor.NextCursor), _nextCursor);
                builder.AddAttribute(524, nameof(PaginationCursor.PreviousCursor), _previousCursor);
                builder.CloseComponent();
            }
            else if (OffsetFetch != null)
            {
                builder.OpenComponent<PaginationOffset>(527);
                builder.AddAttribute(528, nameof(PaginationOffset.CurrentPage), OffsetPaginationState.Page);
                builder.AddAttribute(529, nameof(PaginationOffset.PageChanged), EventCallback.Factory.Create(this, (Func<int, Task>)OnPageChanged));
                builder.AddAttribute(530, nameof(PaginationOffset.TotalItems), _totalItems);
                builder.AddAttribute(531, nameof(PaginationOffset.ItemsPerPage), PageSize ?? OffsetPaginationState.PageSize);
                builder.CloseComponent();
            }

            builder.CloseElement();
        }
    }
}
