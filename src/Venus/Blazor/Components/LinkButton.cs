using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class LinkButton : ButtonBase
{
    [Parameter]
    public int TextStyle { get; set; } = Values.TextStyle.Body;

    [Parameter]
    public int Hierarchy { get; set; } = TextHierarchy.Span;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }


    protected override void BuildButtonClass(List<string> classList)
    {
        base.BuildButtonClass(classList);
        classList.Add("link");
        if (Hierarchy == TextHierarchy.Span)
        {
            classList.Add($"text-{TextStyle.ToString().PascalToKebabCase()}");
        }
    }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Hierarchy == TextHierarchy.Span)
        {
            builder.AddContent(2, ChildContent);
        }
        else
        {
            builder.OpenComponent<TextBlock>(2);
            builder.AddAttribute(3, nameof(TextBlock.Hierarchy), Hierarchy);
            builder.AddAttribute(4, nameof(TextBlock.TextStyle), TextStyle);
            builder.AddAttribute(5, nameof(TextBlock.ChildContent), ChildContent);
            builder.CloseComponent();
        }
    }
}
