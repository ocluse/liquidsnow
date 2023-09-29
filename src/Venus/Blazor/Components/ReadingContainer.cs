using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class ReadingContainer : ControlBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("reading-container");
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.AddMultipleAttributes(1, GetClassAndStyle());

        if (ChildContent != null)
        {
            builder.AddContent(2, ChildContent);
        }

        builder.CloseElement();
    }
}
