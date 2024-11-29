using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A view that displays a single item.
/// </summary>
public class ItemView<T> : DataViewBase
{
    /// <summary>
    /// Gets or sets the content to display when the container has data.
    /// </summary>
    [EditorRequired]
    [Parameter]
    public required RenderFragment<T> ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the item to display.
    /// </summary>
    [Parameter]
    public T? Item { get; set; }

    /// <summary>
    /// Gets or sets a function called when fetching the data.
    /// </summary>
    [Parameter]
    public Func<Task<T>>? Fetch { get; set; }

    ///<inheritdoc/>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        bool reloadRequired = false;

        if (parameters.TryGetValue<Func<Task<T>>?>(nameof(Fetch), out var fetch))
        {
            if (Fetch != fetch)
            {
                reloadRequired = true;
            }
        }

        if (parameters.TryGetValue<T?>(nameof(Item), out var item))
        {
            if (!EqualityComparer<T>.Default.Equals(Item, item))
            {
                reloadRequired = true;
            }
        }

        await base.SetParametersAsync(parameters);

        if (reloadRequired)
        {
            await ReloadDataAsync();
        }
    }

    ///<inheritdoc/>
    protected override void BuildControl(RenderTreeBuilder builder)
    {
        BuildDataContent(builder);
    }

    ///<inheritdoc/>
    protected override void BuildFound(RenderTreeBuilder builder)
    {
        if (Item != null)
        {
            builder.AddContent(1, ChildContent, Item);
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
    protected override async Task<int> FetchDataAsync()
    {
        if (Fetch != null)
        {
            Item = await Fetch.Invoke();
        }

        return Item == null ? ContainerState.NotFound : ContainerState.Found;
    }
}
