using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Blazor.Services;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class GridView<T> : Grid
{
    [Inject]
    public IBlazorResolver ContainerStateResolver { get; set; } = null!;

    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    [Parameter]
    public RenderFragment? ErrorTemplate { get; set; }

    [Parameter]
    public string? ItemClass { get; set; }

    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<T>>>? Fetch { get; set; }

    [Parameter]
    public int State { get; set; }

    [Parameter]
    public string? DisplayMemberPath { get; set; }

    [Parameter]
    public Func<T?, string>? DisplayMemberFunc { get; set; }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Items != null)
        {
            string itemClass = $"grid-item {ItemClass}";
            foreach (var item in Items)
            {
                if (ItemTemplate == null)
                {
                    builder.OpenComponent<TextBlock>(2);
                    builder.SetKey(item);
                    builder.AddAttribute(3, nameof(TextBlock.ChildContent), item.GetDisplayMember(DisplayMemberFunc, DisplayMemberPath));
                    builder.AddAttribute(4, nameof(Class), itemClass);
                    builder.CloseComponent();
                }
                else
                {
                    builder.OpenElement(5, "div");
                    builder.SetKey(item);
                    builder.AddAttribute(6, "class", itemClass);
                    builder.AddContent(7, ItemTemplate, item);
                    builder.CloseElement();
                }
            }
        }
        else
        {
            if (EmptyTemplate != null && State == ContainerState.Empty)
            {
                builder.AddContent(8, EmptyTemplate);
            }
            else if (LoadingTemplate != null && State == ContainerState.Loading)
            {
                builder.AddContent(9, LoadingTemplate);
                builder.CloseComponent();
            }
            else if (ErrorTemplate != null && State == ContainerState.Error)
            {
                builder.AddContent(10, ErrorTemplate);
            }
            else
            {
                Type typeToRender = ContainerStateResolver.ResolveContainerStateToRenderType(State);
                builder.OpenComponent(11, typeToRender);
                builder.CloseComponent();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await ReloadData();
    }

    public async Task ReloadData()
    {
        if (Fetch != null)
        {
            State = ContainerState.Loading;

            try
            {
                Items = await Fetch.Invoke();
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
}
