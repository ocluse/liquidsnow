using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component for displaying brand icons designed under Venus principles.
/// </summary>
public class BrandIcon : ControlBase
{
    /// <summary>
    /// Gets or sets the SVG content of the icon to display.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon in pixels.
    /// </summary>
    [Parameter]
    public int? Size { get; set; }

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
                { "viewBox", "0 0 48 48" },
            };

            builder.AddMultipleAttributes(1, attributes);
            builder.AddMultipleAttributes(2, GetAttributes()!);
            MarkupString content = new(Icon);

            builder.AddContent(3, content);
            builder.CloseElement();
        }
    }
}
