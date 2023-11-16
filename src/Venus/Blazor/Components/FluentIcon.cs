using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class FluentIcon : ControlBase
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public int? Size { get; set; }

    protected virtual int ViewBox { get; } = 24;

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
            builder.AddMultipleAttributes(2, GetClassAndStyle());
            MarkupString content = new(Icon);

            builder.AddContent(3, content);
            builder.CloseElement();
        }
    }
}

public class Fluent32Icon : FluentIcon
{
    protected override int ViewBox { get; } = 32;
}

public class Fluent48Icon : FluentIcon
{
    protected override int ViewBox { get; } = 48;
}

public class Fluent12Icon : FluentIcon
{
    protected override int ViewBox { get; } = 12;
}

public class Fluent16Icon : FluentIcon
{
    protected override int ViewBox { get; } = 16;
}

public class Fluent20Icon : FluentIcon
{
    protected override int ViewBox { get; } = 20;
}

public class Fluent28Icon : FluentIcon
{
    protected override int ViewBox { get; } = 28;
}