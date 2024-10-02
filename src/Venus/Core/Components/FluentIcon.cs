using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component for displaying FluentUI icons.
/// </summary>
public class FluentIcon : ControlBase
{
    /// <summary>
    /// Gets or sets the inner svg content of the icon to display.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of he icon in pixels.
    /// </summary>
    [Parameter]
    public int? Size { get; set; }

    /// <summary>
    /// The size of the view box for the icon.
    /// </summary>
    protected virtual int ViewBox { get; } = 24;

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
                { "viewBox", $"0 0 {ViewBox} {ViewBox}" },
                { "fill", "currentColor" }
            };

            builder.AddMultipleAttributes(1, attributes);
            builder.AddMultipleAttributes(2, GetAttributes());
            MarkupString content = new(Icon);

            builder.AddContent(3, content);
            builder.CloseElement();
        }
    }
}

///<inheritdoc/>
public class Fluent32Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 32;
}

///<inheritdoc/>
public class Fluent48Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 48;
}

///<inheritdoc/>
public class Fluent12Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 12;
}

///<inheritdoc/>
public class Fluent16Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 16;
}

///<inheritdoc/>
public class Fluent20Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 20;
}

///<inheritdoc/>
public class Fluent28Icon : FluentIcon
{
    ///<inheritdoc/>
    protected override int ViewBox { get; } = 28;
}