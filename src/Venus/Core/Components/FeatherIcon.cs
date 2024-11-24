using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders Feather icons and Lucide icons.
/// </summary>
public class FeatherIcon : ControlBase, ISvgIcon
{
    /// <summary>
    /// The default stroke width for Feather icons.
    /// </summary>
    public const int STROKE_WIDTH = 2;

    ///<inheritdoc/>
    [Parameter]
    public string? Icon { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? Size { get; set; }

    /// <summary>
    /// Gets or sets the stroke width of the icon in pixels.
    /// </summary>
    [Parameter]
    public double? StrokeWidth { get; set; }

    /// <summary>
    /// Gets or sets the stroke line cap of the icon.
    /// </summary>
    [Parameter]
    public StrokeLineCap? StrokeLineCap { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public CssUnit? Unit { get; set; }

    /// <summary>
    /// Gets or sets the stroke line join of the icon.
    /// </summary>
    [Parameter]
    public StrokeLineJoin? StrokeLineJoin { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrEmpty(Icon))
        {
            MarkupString content = new(Icon);

            builder.OpenElement(0, "svg");
            {
                builder.AddMultipleAttributes(2, GetAttributes());
                
                builder.AddContent(3, content);
            }
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.FeatherIcon);
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        string size = this.GetIconSize(Resolver);
        base.BuildAttributes(attributes);
        attributes.Add("height", size);
        attributes.Add("width", size);
        attributes.Add("xmlns", "http://www.w3.org/2000/svg");
        attributes.Add("viewBox", "0 0 24 24");
        attributes.Add("fill", "none");
        attributes.Add("stroke", "currentColor");
        attributes.Add("stroke-width", StrokeWidth ?? Resolver.IconStrokeWidth);
        attributes.Add("stroke-linecap", (StrokeLineCap ?? Resolver.FeatherIconStrokeCap).ToHtmlAttributeValue());
        attributes.Add("stroke-linejoin", (StrokeLineJoin ?? Resolver.FeatherIconStrokeLineJoin).ToHtmlAttributeValue());
    }
}
