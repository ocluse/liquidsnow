namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Defines a contract for a component that renders an SVG icon.
/// </summary>
public interface ISvgIcon
{
    /// <summary>
    /// Gets or sets the inner svg content of the icon to display.
    /// </summary>
    string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the size of the icon.
    /// </summary>
    double? Size { get; set; }

    /// <summary>
    /// Gets or sets the unit of the icon size.
    /// </summary>
    CssUnit? Unit { get; set; }
}
