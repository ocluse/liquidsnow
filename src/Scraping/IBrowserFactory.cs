namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Defines methods for creating browser instances.
/// </summary>
public interface IBrowserFactory
{
    /// <summary>
    /// Creates a new browser instance with the specified options.
    /// </summary>
    IBrowser CreateBrowser(BrowserOptions options);
}
