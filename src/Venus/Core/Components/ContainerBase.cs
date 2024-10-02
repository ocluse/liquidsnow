using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A control that hosts data.
/// </summary>
public abstract class ContainerBase : ControlBase
{
    /// <summary>
    /// Gets or sets the state of the container.
    /// </summary>
    [Parameter]
    public int State { get; set; } = ContainerState.Found;

    /// <summary>
    /// Gets or sets a callback that is invoked when the state of the container changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> StateChanged { get; set; }

    /// <summary>
    /// The content to display when the container is empty.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// The content to display when the container is loading data.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// The content to display when an error occurs when loading data.
    /// </summary>
    [Parameter]
    public RenderFragment<Exception?>? ErrorTemplate { get; set; }

    /// <summary>
    /// The content to display when the user is unauthorized to view the data.
    /// </summary>
    [Parameter]
    public RenderFragment? UnauthorizedTemplate { get; set; }

    /// <summary>
    /// The content to display when the data is not found.
    /// </summary>
    [Parameter]
    public RenderFragment? NotFoundTemplate { get; set; }

    /// <summary>
    /// The content to display when the user needs to reauthenticate.
    /// </summary>
    [Parameter]
    public RenderFragment? ReauthenticationRequiredTemplate { get; set; }

    private Exception? _exception;

    /// <summary>
    /// Build the container body in the render tree.
    /// </summary>
    protected void BuildContainer(RenderTreeBuilder builder)
    {
        if (State == ContainerState.Found)
        {
            BuildFound(builder);
        }
        else if (EmptyTemplate != null && State == ContainerState.Empty)
        {
            builder.AddContent(360, EmptyTemplate);
        }
        else if (LoadingTemplate != null && State == ContainerState.Loading)
        {
            builder.AddContent(361, LoadingTemplate);
        }
        else if (State == ContainerState.Error && State == ContainerState.Error)
        {
            builder.AddContent(362, ErrorTemplate, _exception);
        }
        else if (NotFoundTemplate != null && State == ContainerState.NotFound)
        {
            builder.AddContent(363, NotFoundTemplate);
        }
        else if (UnauthorizedTemplate != null && State == ContainerState.Unauthorized)
        {
            builder.AddContent(364, UnauthorizedTemplate);
        }
        else if (ReauthenticationRequiredTemplate != null && State == ContainerState.ReauthenticationRequired)
        {
            builder.AddContent(365, ReauthenticationRequiredTemplate);
        }
        else
        {
            Type typeToRender = Resolver.ResolveContainerStateToComponentType(State);
            builder.OpenComponent(166, typeToRender);
            builder.CloseComponent();
        }
    }

    /// <summary>
    /// Called to build the container when then the data is found.
    /// </summary>
    protected abstract void BuildFound(RenderTreeBuilder builder);

    /// <summary>
    /// A function to fetch teh data to be displayed by the container.
    /// </summary>
    protected abstract Task<int> FetchDataAsync();

    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <returns></returns>
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
    /// Update the state of the container and re-render it.
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
