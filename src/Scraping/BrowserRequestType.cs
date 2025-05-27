namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Defines the type of the simulated browser request.
/// This influences the headers that are set.
/// </summary>
public enum BrowserRequestType
{
    /// <summary>
    /// A Standard page load
    /// </summary>
    Navigation,
    /// <summary>
    /// XMLHttpRequest (Ajax) request.
    /// </summary>
    Xhr,
    /// <summary>
    /// Request for content within an i-frame.
    /// </summary>
    Iframe,
    /// <summary>
    /// Request for loading a JavaScript file.
    /// </summary>
    Script,
    /// <summary>
    /// Request for a CSS file.
    /// </summary>
    Style,
    /// <summary>
    /// Request for an image file.
    /// </summary>
    Image,
    /// <summary>
    /// Request for a font file.
    /// </summary>
    Font,
    /// <summary>
    /// Request for other resource types.
    /// </summary>
    Other
}
