using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays an icon beside a text block.
/// </summary>
public class IconText : ControlBase
{
    /// <summary>
    /// Gets or sets the icon to display.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon being displayed.
    /// </summary>
    /// <remarks>
    /// If not provided, the size will be determined by the TextStyle property.
    /// </remarks>
    [Parameter]
    public int? IconSize { get; set; }

    /// <summary>
    /// Gets or sets the color of the icon.
    /// </summary>
    /// <remarks>
    /// If not provided, the color will be the same as that of the text.
    /// </remarks>
    [Parameter]
    public int? IconColor { get; set; }

    /// <summary>
    /// Gets or sets the style of the icon.
    /// </summary>
    [Parameter]
    public IconStyle? IconStyle { get; set; }

    /// <summary>
    /// Gets or sets the style of the text.
    /// </summary>
    [Parameter]
    public int TextStyle { get; set; } = Values.TextStyle.Body;

    /// <summary>
    /// Gets or sets the inner content of the text block.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.AddMultipleAttributes(1, GetAttributes());
        if (Icon != null)
        {
            var iconStyle = IconStyle ?? Resolver.IconStyle;

            if (iconStyle == Venus.IconStyle.Feather)
            {
                builder.OpenComponent<FeatherIcon>(2);
            }
            else
            {
                builder.OpenComponent<FluentIcon>(3);
            }

            builder.AddAttribute(4, "Icon", Icon);
            builder.AddAttribute(5, "Size", IconSize ?? Resolver.ResolveTextStyleToIconSize(TextStyle));

            if (IconColor != null)
            {
                builder.AddAttribute(6, "Color", IconColor);
            }

            builder.CloseComponent();
        }
        if (ChildContent != null)
        {
            builder.OpenComponent<TextBlock>(7);
            builder.AddAttribute(8, nameof(TextBlock.TextStyle), TextStyle);
            builder.AddAttribute(9, nameof(TextBlock.ChildContent), ChildContent);
            builder.CloseComponent();
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("icon-text");
    }
}
