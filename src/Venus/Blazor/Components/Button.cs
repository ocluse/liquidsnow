using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class Button : ButtonBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildButtonClass(List<string> classList)
    {
        base.BuildButtonClass(classList); 
        classList.Add("button");
    }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.AddContent(2, ChildContent);
    }
}
