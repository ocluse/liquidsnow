using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A control that displays brand icons designed under Venus principles.
/// </summary>
public class BrandIcon : ControlBase, ISvgIcon
{
    ///<inheritdoc/>
    [Parameter]
    public string? Icon { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? Size { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public CssUnit? Unit { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Icon.IsNotEmpty())
        {
            builder.OpenElement(1, "svg");
            {
                builder.AddMultipleAttributes(2, GetAttributes());
                builder.AddContent(3, new MarkupString(Icon));
            }
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);
        string size = this.GetIconSize(Resolver);
        attributes.Add("height", size);
        attributes.Add("width", size);
        attributes.Add("xmlns", "http://www.w3.org/2000/svg");
        attributes.Add("viewBox", "0 0 48 48");
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.BrandIcon);
    }
}
