using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Blazor.Services;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemContainer<T> : ContainerBase
    {
        [EditorRequired]
        [Parameter]
        public required RenderFragment<T> ChildContent { get; set; }

        [Parameter]
        public T? Item { get; set; }

        [Parameter]
        public Func<Task<T>>? Fetch { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReloadData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Fetch == null)
            {
                await ReloadData();
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Item != null)
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

        protected override void BuildFound(RenderTreeBuilder builder)
        {
            if (Item != null)
            {
                builder.AddContent(50, ChildContent, Item);
            }
            else if (EmptyTemplate != null)
            {
                builder.AddContent(51, EmptyTemplate);
            }
            else
            {
                Type typeToRender = Resolver.ResolveContainerStateToRenderType(ContainerState.Empty);
                builder.OpenComponent(51, typeToRender);
                builder.CloseComponent();
            }
        }

        protected override async Task<int> FetchDataAsync()
        {
            if (Fetch != null)
            {
                Item = await Fetch.Invoke();
            }

            return Item == null ? ContainerState.NotFound : ContainerState.Found;
        }
    }
}
