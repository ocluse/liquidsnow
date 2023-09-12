using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class IconButton : ButtonBase
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public int Size { get; set; } = DefaultSize.Size18;

    [Parameter]
    public int StrokeWidth { get; set; } = FeatherIcon.STROKE_WIDTH;

    protected override void BuildButtonClass(List<string> classList)
    {
        base.BuildButtonClass(classList);
        classList.Add("icon-button");
    }
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FeatherIcon>(2);
        builder.AddAttribute(3, nameof(FeatherIcon.Icon), Icon);
        builder.AddAttribute(4, nameof(FeatherIcon.Size), Size);
        builder.AddAttribute(5, nameof(FeatherIcon.StrokeWidth), StrokeWidth);
        builder.CloseComponent();
    }
}
