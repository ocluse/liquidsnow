using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public class Shimmer : ControlBase
{
    [Parameter]
    public ShimmerType Type { get; set; }

    [Parameter]
    public ShimmerLength Length { get; set; }

    [Parameter]
    public ProfileImageSize? ProfileImageSize { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, GetAttributes());
        builder.AddAttribute(2, "role", "status");
        builder.AddAttribute(3, "aria-live", "polite");
        builder.AddAttribute(4, "aria-atomic", "true");
        builder.AddAttribute(5, "aria-busy", "true");
        builder.AddAttribute(6, "aria-hidden", "true");
        builder.CloseElement();
    }

    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add("shimmer")
            .Add(Type.ToString().PascalToKebabCase())
            .Add(Length.ToString().PascalToKebabCase());

        if (ProfileImageSize.HasValue)
        {
            builder.Add(ProfileImageSize.Value.ToString().PascalToKebabCase());
        }
    }
}

