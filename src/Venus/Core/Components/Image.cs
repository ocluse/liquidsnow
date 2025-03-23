namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A control for showing images.
/// </summary>
public class Image : ImageBase
{
    /// <summary>
    /// Gets or sets the width of the image.
    /// </summary>
    [Parameter]
    public double? Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the image.
    /// </summary>
    [Parameter]
    public double? Height { get; set; }

    ///<inheritdoc/>
    protected override double? GetHeight() => Height;

    ///<inheritdoc/>
    protected override double? GetWidth() => Width;
}
