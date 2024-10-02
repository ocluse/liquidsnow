using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that shows a Feather-based icon inside a button.
/// </summary>
public class IconButton : ButtonBase
{
    /// <summary>
    /// Gets or sets the icon to display.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon.
    /// </summary>
    [Parameter]
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets the stroke-width of the icon.
    /// </summary>
    [Parameter]
    public int? StrokeWidth { get; set; }

    ///<inheritdoc/>
    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("icon-button");
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FeatherIcon>(2);
        builder.AddAttribute(3, nameof(FeatherIcon.Icon), Icon);
        builder.AddAttribute(4, nameof(FeatherIcon.Size), Size ?? Resolver.DefaultButtonIconSize);
        builder.AddAttribute(5, nameof(FeatherIcon.StrokeWidth), StrokeWidth ?? Resolver.IconStrokeWidth);
        builder.CloseComponent();
    }
}
