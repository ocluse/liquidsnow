﻿using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web.Virtualization;

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

        [Parameter]
        public bool EnablePagination { get; set; } = true;

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool reloadRequired = false;

            //check page size:
            if (parameters.TryGetValue(nameof(PageSize), out int? pageSize))
            {
                if (PageSize != pageSize)
                {
                    reloadRequired = true;
                }
            }

            //check sort:
            if (parameters.TryGetValue(nameof(Sort), out object? sort))
            {
                if (Sort != sort)
                {
                    reloadRequired = true;
                }
            }

            //check filter:
            if (parameters.TryGetValue(nameof(Filter), out object? filter))
            {
                if (Filter != filter)
                {
                    reloadRequired = true;
                }
            }

            //check ordering:
            if (parameters.TryGetValue(nameof(Ordering), out Ordering? ordering))
            {
                if (Ordering != ordering)
                {
                    reloadRequired = true;
                }
            }

            //check cursor fetch:
            if (parameters.TryGetValue(nameof(CursorFetch), out Func<CursorPaginationState, Task<CursorListViewData<T>>>? cursorFetch))
            {
                if (CursorFetch != cursorFetch)
                {
                    reloadRequired = true;
                }
            }

            //check offset fetch:
            if (parameters.TryGetValue(nameof(OffsetFetch), out Func<OffsetPaginationState, Task<OffsetListViewData<T>>>? offsetFetch))
            {
                if (OffsetFetch != offsetFetch)
                {
                    reloadRequired = true;
                }
            }

            //check cursor:
            if (parameters.TryGetValue(nameof(Cursor), out object? cursor))
            {
                if (Cursor != cursor)
                {
                    reloadRequired = true;
                }
            }

            //check page:
            if (parameters.TryGetValue(nameof(Page), out int? page))
            {
                if (Page != page)
                {
                    reloadRequired = true;
                }
            }

            //check items:
            if (parameters.TryGetValue(nameof(Items), out IEnumerable<T>? items))
            {
                if (Items != items)
                {
                    reloadRequired = true;
                }
            }

            await base.SetParametersAsync(parameters);

            if (reloadRequired)
            {
                await ReloadData();
            }
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
        }

        private async Task OnPageChanged(int newPage)
        {
            await PageChanged.InvokeAsync(newPage);
        }

        protected virtual void RenderFoundCore(RenderTreeBuilder builder, IEnumerable<T> items)
        {
            ClassBuilder containerClass = new();
            StyleBuilder containerStyle = new();

            BuildContainerClass(containerClass);
            BuildContainerStyles(containerStyle);

            builder.OpenElement(50, ContainerElement);
            builder.AddAttribute(51, "class", containerClass.Build());
            builder.AddAttribute(52, "style", containerStyle.Build());

            RenderItems(builder, items);

            builder.CloseElement();
        }

        protected virtual void RenderItems(RenderTreeBuilder builder, IEnumerable<T> items)
        {
            string itemClass = GetItemClass();

            foreach (var item in items)
            {
                builder.OpenElement(53, ItemElement);
                builder.SetKey(item);
                builder.AddAttribute(54, "class", itemClass);
                builder.AddAttribute(55, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
                if (ItemTemplate == null)
                {

                    builder.AddContent(56, item.GetDisplayMember(DisplayMemberFunc, DisplayMemberPath));
                }
                else
                {
                    builder.AddContent(57, ItemTemplate, item);
                }
                builder.CloseElement();
            }
        }

        protected override void BuildFound(RenderTreeBuilder builder)
        {
            if (Items != null && Items.Any())
            {
                RenderFoundCore(builder, Items);
            }
            else if (EmptyTemplate != null)
            {
                builder.AddContent(58, EmptyTemplate);
            }
            else
            {
                Type typeToRender = Resolver.ResolveContainerStateToRenderType(ContainerState.Empty);
                builder.OpenComponent(59, typeToRender);
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

            
            BuildContainer(builder);
            

            if (EnablePagination)
            {
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
                    builder.AddAttribute(531, nameof(PaginationOffset.ItemsPerPage), PageSize ?? Resolver.DefaultPageSize);
                    builder.CloseComponent();
                }
            }

            builder.CloseElement();
        }
    }
}
