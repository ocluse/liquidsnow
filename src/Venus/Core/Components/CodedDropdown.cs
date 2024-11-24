using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;
internal class CodedDropdown<T> : InputBase<T>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenElement(1, "div");
        {
            
        }
        builder.CloseElement();
    }
}
