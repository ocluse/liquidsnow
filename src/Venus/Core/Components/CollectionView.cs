using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Models;
using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays a collection of items.
/// </summary>
/// <typeparam name="T">The type of data displayed by the component</typeparam>
public class CollectionView<T> : ContainerBase, IDisposable
{
    private int _totalItems;

    private object? _nextCursor, _previousCursor;

    private IEnumerable<T>? _renderedItems;

    private bool _disposedValue;

    #region Properties

    /// <summary>
    /// Gets the element that is rendered to contain an item in the collection.
    /// </summary>
    protected virtual string ItemElement => "div";

    /// <summary>
    /// Gets or sets the element that is rendered as the container for the items.
    /// </summary>
    protected virtual string ContainerElement => "div";

    /// <summary>
    /// Gets or sets the items that are currently being rendered.
    /// </summary>
    protected virtual IEnumerable<T>? RenderedItems
    {
        get => _renderedItems;
        set
        {
            if (_renderedItems != value)
            {
                if (_renderedItems is INotifyCollectionChanged oldNotify)
                {
                    oldNotify.CollectionChanged -= OnItemsCollectionChanged;
                }

                _renderedItems = value;

                if (_renderedItems is INotifyCollectionChanged newNotify)
                {
                    newNotify.CollectionChanged += OnItemsCollectionChanged;
                }
            }

        }
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Gets or sets the CSS class applied to the container of the items.
    /// </summary>
    [Parameter]
    public string? ContainerClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to each individual item.
    /// </summary>
    [Parameter]
    public string? ItemClass { get; set; }

    /// <summary>
    /// Gets or sets the filtration options that are used to filter or sort the items.
    /// </summary>
    [Parameter]
    public FiltrationOptions? FiltrationOptions { get; set; }

    /// <summary>
    /// Gets or sets the content that is displayed as the container header.
    /// </summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Gets or sets the template that is used to render each individual item.
    /// </summary>
    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the items to be rendered by the component.
    /// </summary>
    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// Gets or sets a function that is used to fetch the data and the cursors when cursor pagination is in use.
    /// </summary>
    [Parameter]
    public Func<CursorPaginationState, Task<CursorItemsData<T>>>? CursorFetch { get; set; }

    /// <summary>
    /// Gets or sets a function that is used to fetch the data when offset pagination is in use.
    /// </summary>
    [Parameter]
    public Func<OffsetPaginationState, Task<OffsetItemsData<T>>>? OffsetFetch { get; set; }

    /// <summary>
    /// Gets or sets the current cursor when cursor pagination is in use.
    /// </summary>
    [Parameter]
    public object? Cursor { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Cursor"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<object?> CursorChanged { get; set; }

    /// <summary>
    /// Gets or sets the page currently being displayed when offset pagination is in use.
    /// </summary>
    [Parameter]
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Page"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items that can be displayed at a time.
    /// </summary>
    [Parameter]
    public int? PageSize { get; set; }

    /// <summary>
    /// Gets or sets the sorting criteria that is currently in use.
    /// </summary>
    [Parameter]
    public object? Sort { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Sort"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<object?> SortChanged { get; set; }

    /// <summary>
    /// Gets or sets the filtering criteria that is currently in use.
    /// </summary>
    [Parameter]
    public object? Filter { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Filter"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<object?> FilterChanged { get; set; }

    /// <summary>
    /// Gets or sets the ordering that is used to sort the items.
    /// </summary>
    [Parameter]
    public Ordering? Ordering { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the <see cref="Ordering"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<Ordering?> OrderingChanged { get; set; }

    /// <summary>
    /// Gets or sets the property path that is used as the display member.
    /// </summary>
    [Parameter]
    public string? DisplayMemberPath { get; set; }

    /// <summary>
    /// Gets or sets the function that is used to obtain the display member.
    /// </summary>
    [Parameter]
    public Func<T?, string>? DisplayMemberFunc { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when an item in the collection is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<T> ItemClicked { get; set; }
    #endregion

    /// <summary>
    /// Gets or sets whether the default pagination UI should be show.
    /// </summary>
    [Parameter]
    public bool EnablePagination { get; set; } = true;

    /// <summary>
    /// Gets or sets a function that generates the link to the next cursor.
    /// </summary>
    [Parameter]
    public Func<object, string>? CursorLinkGenerator { get; set; }

    /// <summary>
    /// Gets or sets a function that generates the link to different pages.
    /// </summary>
    [Parameter]
    public Func<int, string>? OffsetLinkGenerator { get; set; }

    /// <summary>
    /// Gets or sets a function that generates a link to navigate to when an item is clicked.
    /// </summary>
    [Parameter]
    public Func<T, string>? ItemLinkGenerator { get; set; }

    ///<inheritdoc/>
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
        if (parameters.TryGetValue(nameof(CursorFetch), out Func<CursorPaginationState, Task<CursorItemsData<T>>>? cursorFetch))
        {
            if (CursorFetch != cursorFetch)
            {
                reloadRequired = true;
            }
        }

        //check offset fetch:
        if (parameters.TryGetValue(nameof(OffsetFetch), out Func<OffsetPaginationState, Task<OffsetItemsData<T>>>? offsetFetch))
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

        if (OffsetFetch == null && CursorFetch == null && RenderedItems != Items)
        {
            reloadRequired = true;
        }

        if (reloadRequired)
        {
            await ReloadDataAsync();
        }
    }

    ///<inheritdoc/>
    protected override async Task<int> FetchDataAsync()
    {
        var paginationState = new PaginationState(
            PageSize: PageSize ?? Resolver.DefaultPageSize,
            Sort: Sort ?? FiltrationOptions?.DefaultSort.Value,
            Filter: Filter ?? FiltrationOptions?.DefaultFilter.Value,
            Ordering: Ordering ?? FiltrationOptions?.DefaultSort.Ordering ?? Venus.Ordering.Ascending);

        if (CursorFetch != null)
        {
            CursorPaginationState state = new(
                Cursor,
                paginationState.PageSize,
                paginationState.Sort,
                paginationState.Filter,
                paginationState.Ordering);

            var result = await CursorFetch.Invoke(state);
            RenderedItems = result.Items;
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
            RenderedItems = result.Items;
            _totalItems = result.TotalCount;
        }
        else
        {
            RenderedItems = Items;
        }

        return RenderedItems?.Any() == true ? ContainerState.Found : ContainerState.Empty;
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("items-control");
    }

    /// <summary>
    /// Gets the CSS class that should be applied to each item.
    /// </summary>
    protected virtual string GetItemClass()
    {
        return $"item-view {ItemClass}";
    }

    /// <summary>
    /// Adds the relevant CSS classes to be added to the container.
    /// </summary>
    protected virtual void BuildContainerClass(ClassBuilder builder)
    {
        builder.Add(ContainerClass);
        builder.Add("items-container");
    }

    /// <summary>
    /// Adds the relevant CSS classes to be added to each item.
    /// </summary>
    protected virtual void BuildItemClass(ClassBuilder builder)
    {
        builder.Add(GetItemClass());
    }

    /// <summary>
    /// Allows inheriting classes to add relevant CSS styles that will be added to the container.
    /// </summary>
    protected virtual void BuildContainerStyles(StyleBuilder builder) { }

    private async Task OnFilterChanged(FilterOption? option)
    {
        var newFilter = option?.Value ?? FiltrationOptions?.DefaultFilter.Value;
        await FilterChanged.InvokeAsync(newFilter);
    }

    private async Task OnSortChanged(FilterOption? option)
    {
        var newSort = option?.Value;
        Ordering? ordering = null;

        if (option is SortOption sortOption)
        {
            ordering = sortOption.Ordering;
        }

        await SortChanged.InvokeAsync(newSort);
        await OrderingChanged.InvokeAsync(ordering);
        await ReloadDataAsync();
    }

    private async Task OnCursorChanged(object newCursor)
    {
        await CursorChanged.InvokeAsync(newCursor);
    }

    private async Task OnPageChanged(int newPage)
    {
        await PageChanged.InvokeAsync(newPage);
    }

    /// <summary>
    /// Renders the item container and the items when they have been found.
    /// </summary>
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

    /// <summary>
    /// Renders the items to the container.
    /// </summary>
    protected virtual void RenderItems(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        string itemClass = GetItemClass();

        foreach (var item in items)
        {
            if (ItemLinkGenerator != null)
            {
                builder.OpenElement(53, "a");
                builder.AddAttribute(54, "href", ItemLinkGenerator(item));
            }
            builder.OpenElement(55, ItemElement);
            builder.SetKey(item);
            builder.AddAttribute(56, "class", itemClass);
            builder.AddAttribute(57, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
            if (ItemTemplate == null)
            {

                builder.AddContent(58, item.GetDisplayMemberValue(DisplayMemberFunc, DisplayMemberPath));
            }
            else
            {
                builder.AddContent(59, ItemTemplate, item);
            }
            builder.CloseElement();

            if (ItemLinkGenerator != null)
            {
                builder.CloseElement();
            }
        }
    }

    ///<inheritdoc/>
    protected override void BuildFound(RenderTreeBuilder builder)
    {
        if (RenderedItems != null && RenderedItems.Any())
        {
            RenderFoundCore(builder, RenderedItems);
        }
        else if (EmptyTemplate != null)
        {
            builder.AddContent(58, EmptyTemplate);
        }
        else
        {
            Type typeToRender = Resolver.ResolveContainerStateToComponentType(ContainerState.Empty);
            builder.OpenComponent(59, typeToRender);
            builder.CloseComponent();
        }
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.AddMultipleAttributes(1, GetAttributes());

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
                string filterIcon = Resolver.IconStyle switch
                {
                    IconStyle.Fluent => ComponentIcons.Fluent.Filter,
                    _ => ComponentIcons.Feather.Filter,
                };

                string sortIcon = Resolver.IconStyle switch
                {
                    IconStyle.Fluent => ComponentIcons.Fluent.Sort,
                    _ => ComponentIcons.Feather.Sort,
                };

                builder.OpenElement(7, "div");
                builder.AddAttribute(8, "class", "filtration-content");
                builder.OpenComponent<FilterDropdown>(9);
                builder.AddAttribute(10, nameof(FilterDropdown.Icon), filterIcon);
                builder.AddAttribute(11, nameof(FilterDropdown.Value), Filter);
                builder.AddAttribute(12, nameof(FilterDropdown.Placeholder), "Filter By");
                builder.AddAttribute(13, nameof(FilterDropdown.ValueChanged), OnFilterChanged);
                builder.CloseComponent();

                builder.OpenComponent<FilterDropdown>(14);
                builder.AddAttribute(15, nameof(FilterDropdown.Icon), sortIcon);
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
                if (CursorLinkGenerator != null)
                {
                    builder.AddAttribute(525, nameof(PaginationCursor.LinkGenerator), (object)CursorLinkGenerator);
                }
                builder.CloseComponent();
            }
            else if (OffsetFetch != null)
            {
                builder.OpenComponent<PaginationOffset>(527);
                builder.AddAttribute(528, nameof(PaginationOffset.CurrentPage), Page);
                builder.AddAttribute(529, nameof(PaginationOffset.PageChanged), EventCallback.Factory.Create(this, (Func<int, Task>)OnPageChanged));
                builder.AddAttribute(530, nameof(PaginationOffset.TotalItems), _totalItems);
                builder.AddAttribute(531, nameof(PaginationOffset.PageSize), PageSize ?? Resolver.DefaultPageSize);
                if (OffsetLinkGenerator != null)
                {
                    builder.AddAttribute(532, nameof(PaginationOffset.LinkGenerator), (object)OffsetLinkGenerator);
                }
                builder.CloseComponent();
            }
        }

        builder.CloseElement();
    }

    private async void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await ReloadDataAsync();
    }

    ///<inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue && disposing)
        {
            if (RenderedItems is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged -= OnItemsCollectionChanged;
            }

            _disposedValue = true;
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
