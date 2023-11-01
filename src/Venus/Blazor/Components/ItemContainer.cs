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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool reloadRequired = false;

            if (parameters.TryGetValue<Func<Task<T>>?>(nameof(Fetch), out var fetch))
            {
                if(Fetch != fetch)
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
