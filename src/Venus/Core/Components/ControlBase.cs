namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for all Venus controls.
/// </summary>
public class ControlBase : ElementBase
{
    /// <summary>
    /// Gets or sets the color of the control.
    /// </summary>
    [Parameter]
    public int? Color { get; set; }

    /// <summary>
    /// Gets or sets the background color of the control.
    /// </summary>
    [Parameter]
    public int? BackgroundColor { get; set; }

    ///<inheritdoc/>
    protected override void BuildStyle(StyleBuilder styleBuilder)
    {
        base.BuildStyle(styleBuilder);

        if (Color != null)
        {
            styleBuilder.Add("color", Resolver.ResolveColor(Color.Value));
        }

        if (BackgroundColor != null)
        {
            styleBuilder.Add("background-color", Resolver.ResolveColor(BackgroundColor.Value));
        }
    }
}
