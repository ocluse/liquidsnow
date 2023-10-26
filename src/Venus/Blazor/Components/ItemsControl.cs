using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemsControl<T> : ContainerBase
    {
        private int _totalItems;

        private object? _nextCursor, _previousCursor;

        #region Properties

        protected virtual string ItemElement { get; } = "div";

        protected virtual string ContainerElement { get; } = "div";

        #endregion

        #region Parameters

        [Parameter]
        public string? ContainerClass { get; set; }

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
        public EventCallback<IEnumerable<T>?> ItemsChanged { get; set; }

        [Parameter]
        public Func<CursorPaginationState, Task<CursorListViewData<T>>>? CursorFetch { get; set; }

        [Parameter]
        public Func<OffsetPaginationState, Task<OffsetListViewData<T>>>? OffsetFetch { get; set; }

        [Parameter]
        public object? Cursor { get; set; }

        [Parameter]
        public EventCallback<object?> CursorChanged { get; set; }

        [Parameter]
        public int Page { get; set; }

        [Parameter]
        public EventCallback<int> PageChanged { get; set; }

        [Parameter]
        public int? PageSize { get; set; }

        [Parameter]
        public object? Sort { get; set; }

        [Parameter]
        public EventCallback<object?> SortChanged { get; set; }

        [Parameter]
        public object? Filter { get; set; }

        [Parameter]
        public EventCallback<object?> FilterChanged { get; set; }

        [Parameter]
        public Ordering? Ordering { get; set; }

        [Parameter]
        public EventCallback<Ordering?> OrderingChanged { get; set; }

        [Parameter]
        public string? DisplayMemberPath { get; set; }

        [Parameter]
        public Func<T?, string>? DisplayMemberFunc { get; set; }

        [Parameter]
        public EventCallback<T> ItemClicked { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            PageSize ??= Resolver.DefaultPageSize;
            Sort ??= FiltrationOptions?.DefaultSort.Value;
            Filter ??= FiltrationOptions?.DefaultFilter.Value;
            Ordering ??= FiltrationOptions?.DefaultSort.Ordering ?? Blazor.Ordering.Ascending;

            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await ReloadData();
        }

        protected override async Task<int> FetchDataAsync()
        {
            var paginationState = new PaginationState(
                PageSize: PageSize ?? Resolver.DefaultPageSize,
                Sort: Sort ?? FiltrationOptions?.DefaultSort.Value,
                Filter: Filter ?? FiltrationOptions?.DefaultFilter.Value,
                Ordering: Ordering ?? FiltrationOptions?.DefaultSort.Ordering ?? Blazor.Ordering.Ascending);

            if (CursorFetch != null)
            {
                CursorPaginationState state = new(
                    Cursor,
                    paginationState.PageSize,
                    paginationState.Sort,
                    paginationState.Filter,
                    paginationState.Ordering);

                var result = await CursorFetch.Invoke(state);
                Items = result.Items;
                _nextCursor = result.NextCursor;
                _previousCursor = result.PreviousCursor;
            }
            else if (OffsetFetch != null)
            {
                OffsetPaginationState state = new(
                    Page,
                    paginationState.PageSize,
                    paginationState.Sort,
                    paginationState.Filter,
                    paginationState.Ordering);

                var result = await OffsetFetch.Invoke(state);
                Items = result.Items;
                _totalItems = result.TotalCount;
            }

            return Items?.Any() == true ? ContainerState.Found : ContainerState.Empty;
        }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add("items-control");
        }

        protected virtual string GetItemClass()
        {
            return $"item-view {ItemClass}";
        }

        protected virtual void BuildContainerClass(ClassBuilder builder)
        {
            builder.Add(ContainerClass);
            builder.Add("items-container");
        }

        protected virtual void BuildItemClass(ClassBuilder builder)
        {
            builder.Add(GetItemClass());
        }

        protected virtual void BuildContainerStyles(StyleBuilder builder)
        {
        }

        private async Task OnFilterChanged(FilterOption? option)
        {
            var newFilter = option?.Value ?? FiltrationOptions?.DefaultFilter.Value;
            await FilterChanged.InvokeAsync(newFilter);
            await ReloadData();
        }

        public async Task OnSortChanged(FilterOption? option)
        {
            var newSort = option?.Value;
            Ordering? ordering = null;

            if (option is SortOption sortOption)
            {
                ordering = sortOption.Ordering;
            }

            await SortChanged.InvokeAsync(newSort);
            await OrderingChanged.InvokeAsync(ordering);
            await ReloadData();
        }

        private async Task OnCursorChanged(object newCursor)
        {
            await CursorChanged.InvokeAsync(newCursor);
            await ReloadData();
        }

        private async Task OnPageChanged(int newPage)
        {
            await PageChanged.InvokeAsync(newPage);
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

            builder.AddMultipleAttributes(1, GetClassAndStyle());

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
                    builder.AddAttribute(11, nameof(FilterDropdown.Value), Filter);
                    builder.AddAttribute(12, nameof(FilterDropdown.Placeholder), "Filter By");
                    builder.AddAttribute(13, nameof(FilterDropdown.ValueChanged), OnFilterChanged);
                    builder.CloseComponent();

                    builder.OpenComponent<FilterDropdown>(14);
                    builder.AddAttribute(15, nameof(FilterDropdown.Icon), FeatherIcons.List);
                    builder.AddAttribute(16, nameof(FilterDropdown.Value), Sort);
                    builder.AddAttribute(17, nameof(FilterDropdown.Placeholder), "Sort By");
                    builder.AddAttribute(18, nameof(FilterDropdown.ValueChanged), OnSortChanged);
                    builder.CloseComponent();
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            ClassBuilder containerClass = new();
            StyleBuilder containerStyle = new();

            BuildContainerClass(containerClass);
            BuildContainerStyles(containerStyle);

            builder.OpenElement(20, ContainerElement);
            builder.AddAttribute(21, "class", containerClass.Build());
            builder.AddAttribute(22, "style", containerStyle.Build());
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
                builder.AddAttribute(528, nameof(PaginationOffset.CurrentPage), Page);
                builder.AddAttribute(529, nameof(PaginationOffset.PageChanged), EventCallback.Factory.Create(this, (Func<int, Task>)OnPageChanged));
                builder.AddAttribute(530, nameof(PaginationOffset.TotalItems), _totalItems);
                builder.AddAttribute(531, nameof(PaginationOffset.ItemsPerPage), PageSize);
                builder.CloseComponent();
            }

            builder.CloseElement();
        }
    }
}
