using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class ListView<T> : ControlBase
{
    private FilterOption? _selectedFilterOption, _selectedSortOption;

    private int _totalItems;
    
    private object? _nextCursor, _previousCursor;

    [Inject]
    public IBlazorResolver ContainerStateResolver { get; set; } = null!;

    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public FiltrationOptions? FiltrationOptions { get; set; }

    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }
    
    [Parameter]
    public IEnumerable<T>? Items { get; set; }
    
    [Parameter]
    public Func<CursorPaginationState, Task<CursorListViewData<T>>>? CursorFetch { get; set; }

    [Parameter]
    public Func<OffsetPaginationState, Task<OffsetListViewData<T>>>? OffsetFetch { get; set; }

    [Parameter]
    public string? ItemClass { get; set; }

    [Parameter]
    public int State { get; set; }
    
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
    public EventCallback<T> ItemClicked { get; set; }

    [Parameter]
    public Func<T?, string>? DisplayMemberFunc { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        if(Header != null || FiltrationOptions != null)
        {
            builder.OpenElement(1, "list-header");

            if (Header != null)
            {
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "header-content");
                builder.AddContent(4, Header);
                builder.CloseElement();
            }
           
            if(FiltrationOptions != null)
            {
                builder.OpenElement(5, "div");
                builder.AddAttribute(6, "class", "list-filtration");
                builder.OpenComponent<FilterDropdown>(7);
                builder.AddAttribute(8, nameof(FilterDropdown.Icon), FeatherIcons.Filter);
                builder.AddAttribute(9, nameof(FilterDropdown.Value), _selectedFilterOption);
                builder.AddAttribute(10, nameof(FilterDropdown.Placeholder), "Filter By");
                builder.AddAttribute(11, nameof(FilterDropdown.ValueChanged), FilterChanged);
                builder.CloseComponent();

                builder.OpenComponent<FilterDropdown>(12);
                builder.AddAttribute(13, nameof(FilterDropdown.Icon), FeatherIcons.List);
                builder.AddAttribute(14, nameof(FilterDropdown.Value), _selectedSortOption);
                builder.AddAttribute(15, nameof(FilterDropdown.Placeholder), "Sort By");
                builder.AddAttribute(16, nameof(FilterDropdown.ValueChanged), SorterChanged);
                builder.CloseComponent();
            }

            builder.CloseElement();
        }

        Dictionary<string, object> attributes = new()
        {
            { "class", GetClass() },
            {"style", GetStyle() },
        };

        builder.AddMultipleAttributes(17, attributes);

        BuildContent(builder);

        builder.CloseElement();

        if (CursorFetch != null)
        {
            builder.OpenComponent<PaginationCursor>(120);
            builder.AddAttribute(122, nameof(PaginationCursor.CursorChanged), EventCallback.Factory.Create(this, OnCursorChanged));
            builder.AddAttribute(123, nameof(PaginationCursor.NextCursor), _nextCursor);
            builder.AddAttribute(124, nameof(PaginationCursor.PreviousCursor), _previousCursor);
            builder.CloseComponent();
        }
        else if (OffsetFetch != null)
        {
            builder.OpenComponent<PaginationOffset>(127);
            builder.AddAttribute(128, nameof(PaginationOffset.CurrentPage), OffsetPaginationState.Page);
            builder.AddAttribute(129, nameof(PaginationOffset.PageChanged), EventCallback.Factory.Create(this, (Func<int,Task>)OnPageChanged));
            builder.AddAttribute(130, nameof(PaginationOffset.TotalItems), _totalItems);
            builder.AddAttribute(131, nameof(PaginationOffset.ItemsPerPage), PageSize ?? OffsetPaginationState.PageSize);
            builder.CloseComponent();
        }
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        if (Items != null)
        {
            string itemClass = $"list-item {ItemClass}";
            foreach (var item in Items)
            {

                if (ItemTemplate == null)
                {
                    builder.OpenComponent<TextBlock>(52);
                    builder.SetKey(item);
                    builder.AddAttribute(53, nameof(TextBlock.ChildContent), item.GetDisplayMember(DisplayMemberFunc, DisplayMemberPath));
                    builder.AddAttribute(54, nameof(Class), itemClass);
                    builder.AddAttribute(55, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
                    builder.CloseComponent();
                }
                else
                {
                    builder.OpenElement(56, "div");
                    builder.SetKey(item);
                    builder.AddAttribute(57, "class", itemClass);
                    builder.AddAttribute(58, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
                    builder.AddContent(59, ItemTemplate, item);
                    builder.CloseElement();
                }
            }
        }
        else
        {
            Type typeToRender = ContainerStateResolver.ResolveContainerStateToRenderType(State);
            builder.OpenComponent(58, typeToRender);
            builder.CloseComponent();
        }
    }

    protected override void BuildClass(List<string> classList)
    {
        base.BuildClass(classList);
        classList.Add("list");
    }

    protected override async Task OnInitializedAsync()
    {
        await ReloadData();
    }
    
    public async Task ReloadData()
    {
        if (CursorFetch != null)
        {
            State = ContainerState.Loading;

            try
            {
                var result = await CursorFetch.Invoke(CursorPaginationState);
                Items = result.Items;
                _nextCursor = result.NextCursor;
                _previousCursor = result.PreviousCursor;
            }
            catch (Exception ex)
            {
                State = VenusResolver.ResolveExceptionToContainerState(ex);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }
        else if (OffsetFetch != null)
        {
            State = ContainerState.Loading;

            try
            {
                var state = OffsetPaginationState.Copy();
                state.WithPageSize(PageSize ?? state.PageSize);
                var result = await OffsetFetch.Invoke(state);
                Items = result.Items;
                _totalItems = result.TotalItems;
            }
            catch (Exception ex)
            {
                State = VenusResolver.ResolveExceptionToContainerState(ex);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }
    }

    private Task FilterChanged(FilterOption? option)
    {
        _selectedFilterOption = option;
        object? filter = option?.Value ?? FiltrationOptions?.DefaultFilter.Value;
        var newCursorState = CursorPaginationState.Copy().WithFilter(filter);
        var newOffsetState = OffsetPaginationState.Copy().WithFilter(filter);
        return SetState(newOffsetState, newCursorState);
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

        return SetState(newOffsetState, newCursorState);
    }

    private async Task SetState(OffsetPaginationState offsetPagination, CursorPaginationState cursorPagination)
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

    private Task OnCursorChanged(object newCursor)
    {
        var newCursorState = CursorPaginationState.Copy().WithCursor(newCursor);
        return SetState(OffsetPaginationState, newCursorState);
    }

    private Task OnPageChanged(int newPage)
    {
        var newOffsetState = OffsetPaginationState.Copy().WithPage(newPage);
        return SetState(newOffsetState, CursorPaginationState);
    }
}
