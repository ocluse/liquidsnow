using Microsoft.AspNetCore.Components.Rendering;

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
            BuildContainer(builder);
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
