using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Blazor.Services;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemContainer<T> : ControlBase
    {
        [Inject]
        public IBlazorResolver Resolver { get; set; } = null!;

        [EditorRequired]
        [Parameter]
        public required RenderFragment<T> ChildContent { get; set; }

        [Parameter]
        public RenderFragment? EmptyTemplate { get; set; }

        [Parameter]
        public RenderFragment? LoadingTemplate { get; set; }

        [Parameter]
        public RenderFragment? ErrorTemplate { get; set; }

        [Parameter]
        public T? Item { get; set; }

        [Parameter]
        public Func<Task<T>>? Fetch { get; set; }

        [Parameter]
        public int State { get; set; }

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
                    Item = await Fetch.Invoke();
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

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if(Item != null)
            {
                builder.AddContent(0, ChildContent, Item);
            }
            else
            {
                if (EmptyTemplate != null && State == ContainerState.Empty)
                {
                    builder.AddContent(1, EmptyTemplate);
                }
                else if (LoadingTemplate != null && State == ContainerState.Loading)
                {
                    builder.AddContent(2, LoadingTemplate);
                    builder.CloseComponent();
                }
                else if (ErrorTemplate != null && State == ContainerState.Error)
                {
                    builder.AddContent(3, ErrorTemplate);
                }
                else
                {
                    Type typeToRender = Resolver.ResolveContainerStateToRenderType(State);
                    builder.OpenComponent(4, typeToRender);
                    builder.CloseComponent();
                }
            }
        }
    }
}
