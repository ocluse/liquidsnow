using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base component for controls that render interface based on data.
/// </summary>
public abstract class DataViewBase : ControlBase, IDataView
{
    /// <summary>
    /// Gets or sets the state of the container.
    /// </summary>
    [Parameter]
    public int State { get; set; } = ContainerState.Found;

    /// <summary>
    /// Gets or sets a callback that is invoked when the state of the component changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> StateChanged { get; set; }

    /// <summary>
    /// Gets or sets the content to display when the component is empty.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Gets or sets the content to display when the component is fetching data.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Gets or sets the content to display when an error occurs when fetching data.
    /// </summary>
    [Parameter]
    public RenderFragment<Exception?>? ErrorTemplate { get; set; }

    /// <summary>
    /// Gets or sets the content to display when the user is unauthorized to view the data.
    /// </summary>
    [Parameter]
    public RenderFragment? UnauthorizedTemplate { get; set; }

    /// <summary>
    /// Gets or sets the content to display when the data is not found.
    /// </summary>
    [Parameter]
    public RenderFragment? NotFoundTemplate { get; set; }

    /// <summary>
    /// Gets or sets the content to display when the user needs to reauthenticate.
    /// </summary>
    [Parameter]
    public RenderFragment? ReauthenticationRequiredTemplate { get; set; }

    private Exception? _exception;

    ///<inheritdoc/>
    protected override sealed void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenComponent<CascadingValue<IDataView>>(1);
        {
            builder.AddAttribute(2, nameof(CascadingValue<IDataView>.Value), this);
            builder.AddAttribute(3, nameof(CascadingValue<IDataView>.IsFixed), true);
            builder.AddAttribute(4, nameof(CascadingValue<IDataView>.ChildContent), (RenderFragment)BuildControl);
        }
        builder.CloseComponent();
    }

    /// <summary>
    /// Renders the rest of the control to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    protected abstract void BuildControl(RenderTreeBuilder builder);

    /// <summary>
    /// Renders the data content to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    protected void BuildDataContent(RenderTreeBuilder builder)
    {
        if (State == ContainerState.Found)
        {
            builder.OpenRegion(1);
            {
                BuildFound(builder);
            }
            builder.CloseRegion();
        }
        else if (EmptyTemplate != null && State == ContainerState.Empty)
        {
            builder.AddContent(2, EmptyTemplate);
        }
        else if (LoadingTemplate != null && State == ContainerState.Loading)
        {
            builder.AddContent(3, LoadingTemplate);
        }
        else if (State == ContainerState.Error && State == ContainerState.Error)
        {
            builder.AddContent(4, ErrorTemplate, _exception);
        }
        else if (NotFoundTemplate != null && State == ContainerState.NotFound)
        {
            builder.AddContent(5, NotFoundTemplate);
        }
        else if (UnauthorizedTemplate != null && State == ContainerState.Unauthorized)
        {
            builder.AddContent(6, UnauthorizedTemplate);
        }
        else if (ReauthenticationRequiredTemplate != null && State == ContainerState.ReauthenticationRequired)
        {
            builder.AddContent(7, ReauthenticationRequiredTemplate);
        }
        else
        {
            Type typeToRender = Resolver.ResolveContainerStateToComponentType(State);
            builder.OpenComponent(8, typeToRender);
            builder.CloseComponent();
        }
    }

    /// <summary>
    /// Renders the found state to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    protected abstract void BuildFound(RenderTreeBuilder builder);

    /// <summary>
    /// Fetches the data to display and returns the appropriate container state.
    /// </summary>
    protected abstract Task<int> FetchDataAsync();

    ///<inheritdoc/>
    public async Task ReloadDataAsync()
    {
        await UpdateStateAsync(ContainerState.Loading);
        _exception = null;
        int newState;

        try
        {
            newState = await FetchDataAsync();
        }
        catch (Exception ex)
        {
            _exception = ex;

            if (ErrorTemplate != null)
            {
                newState = ContainerState.Error;
            }
            else
            {
                newState = Resolver.ResolveExceptionToContainerState(ex);
            }
        }

        await UpdateStateAsync(newState);
    }

    /// <summary>
    /// Updates the container state and renders the new state.
    /// </summary>
    protected async Task UpdateStateAsync(int newState)
    {
        if (newState != State)
        {
            State = newState;
            await InvokeAsync(StateHasChanged);
        }
    }
}
