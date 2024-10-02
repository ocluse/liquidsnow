using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that shows Feather icons and Lucide icons.
/// </summary>
public class FeatherIcon : ControlBase
{
    /// <summary>
    /// The default stroke width for Feather icons.
    /// </summary>
    public const int STROKE_WIDTH = 2;

    /// <summary>
    /// The inner svg content of the icon to display.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// The size, in pixels, of the icon.
    /// </summary>
    [Parameter]
    public int? Size { get; set; }

    /// <summary>
    /// The stroke width of the icon in pixels.
    /// </summary>
    [Parameter]
    public int? StrokeWidth { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrEmpty(Icon))
        {
            builder.OpenElement(0, "svg");

            Dictionary<string, object> attributes = new()
            {
                { "height", Size ?? Resolver.DefaultIconSize},
                { "width", Size ?? Resolver.DefaultIconSize},
                { "xmlns", "http://www.w3.org/2000/svg" },
                { "viewBox", "0 0 24 24" },
                { "fill", "none" },
                { "stroke", "currentColor" },
                { "stroke-width",StrokeWidth ?? Resolver.IconStrokeWidth },
                { "stroke-linecap","round"},
                { "stroke-linejoin", "round"}
            };

            builder.AddMultipleAttributes(1, attributes);
            builder.AddMultipleAttributes(2, GetAttributes());

            MarkupString content = new(Icon);

            builder.AddContent(3, content);
            builder.CloseElement();
        }
    }
}
