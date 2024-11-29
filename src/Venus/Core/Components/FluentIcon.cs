using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders FluentUI icons.
/// </summary>
public class FluentIcon : ControlBase, ISvgIcon
{
    ///<inheritdoc/>
    [Parameter]
    public string? Icon { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public CssUnit? Unit { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? Size { get; set; }

    /// <summary>
    /// The size of the view box for the icon.
    /// </summary>
    protected virtual int ViewBox { get; } = 24;

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrEmpty(Icon))
        {
            MarkupString content = new(Icon);

            builder.OpenElement(0, "svg");
            {
                builder.AddMultipleAttributes(1, GetAttributes());

                builder.AddContent(2, content);
            }            
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        string size = this.GetIconSize(Resolver);

        base.BuildAttributes(attributes);
        attributes.Add("height", size);
        attributes.Add("width", size);
        attributes.Add("xmlns", "http://www.w3.org/2000/svg");
        attributes.Add("viewBox", $"0 0 {ViewBox} {ViewBox}");
        attributes.Add("fill", "currentColor");
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.FluentIcon);
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