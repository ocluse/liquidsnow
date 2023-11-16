using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class IconButton : ButtonBase
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public int? Size { get; set; }

    [Parameter]
    public int? StrokeWidth { get; set; }

    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("icon-button");
    }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FeatherIcon>(2);
        builder.AddAttribute(3, nameof(FeatherIcon.Icon), Icon);
        builder.AddAttribute(4, nameof(FeatherIcon.Size), Size ?? Resolver.DefaultButtonIconSize);
        builder.AddAttribute(5, nameof(FeatherIcon.StrokeWidth), StrokeWidth ?? Resolver.DefaultFeatherStokeWidth);
        builder.CloseComponent();
    }
}
