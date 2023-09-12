using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class ReadingContainer : ControlBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildClass(List<string> classList)
    {
        base.BuildClass(classList);
        classList.Add("reading-container");
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        Dictionary<string, object> attributes = new()
        {
            { "class",GetClass()},
            {"style", GetStyle()},
        };

        builder.AddMultipleAttributes(1, attributes);

        if (ChildContent != null)
        {
            builder.AddContent(2, ChildContent);
        }

        builder.CloseElement();
    }
}
