using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Internal.ContainerStates;

internal sealed class Empty : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, "There are no items to display");
    }
}
