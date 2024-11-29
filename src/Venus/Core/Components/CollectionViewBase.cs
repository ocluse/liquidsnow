using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for components that display a collection of items.
/// </summary>
/// <typeparam name="T">The type of data displayed by the component</typeparam>
public abstract class CollectionViewBase<T> : DataViewBase, ICollectionView<T>, IDisposable
{
    private IEnumerable<T>? _renderedItems;

    private bool _disposedValue;

    #region Properties

    /// <summary>
    /// Gets the element that is rendered to contain an single item in the collection.
    /// </summary>
    protected virtual string ItemElement => "div";

    /// <summary>
    /// Gets the element that is rendered to contain all items in the collection.
    /// </summary>
    protected virtual string ContainerElement => "div";

    /// <summary>
    /// Gets or sets the actual items that are supposed to be rendered.
    /// </summary>
    protected virtual IEnumerable<T>? RenderedItems
    {
        get => _renderedItems;
        private set
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
    /// Gets or sets the content that is displayed as the header.
    /// </summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the content that is displayed as the footer.
    /// </summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the footer.
    /// </summary>
    [Parameter]
    public string? FooterClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// Gets or sets a function that is used to fetch items asynchronously.
    /// </summary>
    [Parameter]
    public Func<Task<IEnumerable<T>>>? Fetch { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public Func<T?, string>? ToStringFunc { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when an item in the collection is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<T> ItemClicked { get; set; }
    #endregion

    /// <summary>
    /// Gets or sets a function that generates a link to navigate to when an item is clicked.
    /// </summary>
    [Parameter]
    public Func<T, string>? ItemLinkGenerator { get; set; }

    ///<inheritdoc/>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        bool reloadRequired = false;

        //check the standard fetch:
        if (parameters.TryGetValue(nameof(Fetch), out Func<Task<IEnumerable<T>>>? fetch))
        {
            if (Fetch != fetch)
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

        if (Fetch == null && RenderedItems != Items)
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
        if (Fetch != null)
        {
            RenderedItems = await Fetch();
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
        classBuilder.Add(ClassNameProvider.CollectionView);
    }

    /// <summary>
    /// Returns the CSS class that should be applied to each item.
    /// </summary>
    protected string GetItemClass()
    {
        ClassBuilder builder = new();
        builder.Add(ClassNameProvider.CollectionView_Item);
        BuildItemClass(builder);
        builder.Add(ItemClass);
        return builder.Build();
    }

    /// <summary>
    /// Adds CSS classes to be added to each item to the supplied <see cref="ClassBuilder"/>.
    /// </summary>
    protected virtual void BuildItemClass(ClassBuilder builder) { }

    /// <summary>
    /// Returns the CSS class that should be applied to the container hosting the items.
    /// </summary>
    protected string GetContainerClass()
    {
        ClassBuilder builder = new();
        builder.Add(ClassNameProvider.CollectionView_ItemsContainer);
        BuildContainerClass(builder);
        builder.Add(ContainerClass);
        return builder.Build();
    }

    /// <summary>
    /// Adds CSS classes to be added to the container to the supplied <see cref="ClassBuilder"/>.
    /// </summary>
    protected virtual void BuildContainerClass(ClassBuilder builder) { }

    /// <summary>
    /// Gets the CSS styles to be added to the container
    /// </summary>
    /// <returns></returns>
    protected string GetContainerStyles()
    {
        StyleBuilder containerStyle = new();
        BuildContainerStyles(containerStyle);
        return containerStyle.Build();
    }

    /// <summary>
    /// Adds CSS styles that will be added to the container to the supplied <see cref="StyleBuilder"/>.
    /// </summary>
    protected virtual void BuildContainerStyles(StyleBuilder builder) { }

    /// <summary>
    /// Renders the item container to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    protected virtual void BuildContainer(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        builder.OpenElement(1, ContainerElement);
        {
            builder.AddAttribute(2, "class", GetContainerClass());
            builder.AddAttribute(3, "style", GetContainerStyles());
            builder.OpenRegion(4);
            {
                BuildItems(builder, items);
            }
            builder.CloseRegion();
        }
        builder.CloseElement();
    }

    /// <summary>
    /// Renders the items to the container in the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    protected virtual void BuildItems(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        string itemClass = GetItemClass();

        foreach (var item in items)
        {
            if (ItemLinkGenerator != null)
            {
                builder.OpenElement(1, "a");
                builder.AddAttribute(2, "href", ItemLinkGenerator(item));
            }
            {
                builder.OpenElement(3, ItemElement);
                {
                    builder.SetKey(item);
                    builder.AddAttribute(4, "class", itemClass);
                    builder.AddAttribute(5, "onclick", EventCallback.Factory.Create(this, async () => { await ItemClicked.InvokeAsync(item); }));
                    if (ItemTemplate == null)
                    {
                        builder.OpenElement(6, "span");
                        {
                            builder.AddContent(7, item.GetDisplayValue(ToStringFunc));
                        }
                        builder.CloseElement();
                    }
                    else
                    {
                        builder.AddContent(8, ItemTemplate, item);
                    }
                }
                builder.CloseElement();
            }
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
            builder.OpenRegion(1);
            {
                BuildContainer(builder, RenderedItems);
            }
            builder.CloseRegion();
        }
        else if (EmptyTemplate != null)
        {
            builder.AddContent(2, EmptyTemplate);
        }
        else
        {
            Type typeToRender = Resolver.ResolveContainerStateToComponentType(ContainerState.Empty);
            builder.OpenComponent(3, typeToRender);
            builder.CloseComponent();
        }
    }

    ///<inheritdoc/>
    protected override void BuildControl(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            if (Header != null)
            {
                builder.OpenElement(3, "div");
                {
                    builder.AddAttribute(4, "class", ClassBuilder.Join(ClassNameProvider.CollectionView_Header, HeaderClass));
                    builder.AddContent(5, Header);
                }
                builder.CloseElement();
            }

            builder.OpenRegion(6);
            {
                BuildDataContent(builder);
            }
            builder.CloseRegion();

            if (Footer != null)
            {
                builder.OpenElement(7, "div");
                {
                    builder.AddAttribute(8, "class", ClassBuilder.Join(ClassNameProvider.CollectionView_Footer, FooterClass));
                    builder.AddContent(9, Footer);
                }
                builder.CloseElement();
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
