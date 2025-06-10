using AngleSharp;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Options used to configure the browser.
/// </summary>
public class BrowserOptions
{
    /// <summary>
    /// Gets or sets the User-Agent string to use.
    /// </summary>
    public string UserAgent { get; set; } = BrowserHeaders.DefaultUserAgent;

    /// <summary>
    /// The Accept-Language string to use.
    /// </summary>
    public string? AcceptLanguage { get; set; } = BrowserHeaders.DefaultAcceptLanguage;

    /// <summary>
    /// Gets or sets the Accept header for navigation and iframe requests.
    /// </summary>
    public string? AcceptHeaderNavigationAndIframe { get; set; } = BrowserHeaders.AcceptHeaderNavigationAndIframe;

    /// <summary>
    /// Gets or sets the Accept header for XMLHttpRequest (XHR) requests.
    /// </summary>
    public string? AcceptHeaderXhr { get; set; } = BrowserHeaders.AcceptHeaderXhr;

    /// <summary>
    /// Gets or sets the Accept header for script requests.
    /// </summary>
    public string? AcceptHeaderScript { get; set; } = BrowserHeaders.AcceptHeaderScript;

    /// <summary>
    /// Gets or sets the Accept header for style (CSS) requests.
    /// </summary>
    public string? AcceptHeaderStyle { get; set; } = BrowserHeaders.AcceptHeaderStyle;

    /// <summary>
    /// Gets or sets the Accept header for image requests.
    /// </summary>
    public string? AcceptHeaderImage { get; set; } = BrowserHeaders.AcceptHeaderImage;

    /// <summary>
    /// Gets or sets the Accept header for font requests.
    /// </summary>
    public string? AcceptHeaderFont { get; set; } = BrowserHeaders.AcceptHeaderFont;

    /// <summary>
    /// Gets or sets the browsing context used to load html documents.
    /// </summary>
    public IBrowsingContext BrowsingContext { get; set; } = AngleSharp.BrowsingContext.New(Configuration.Default.WithDefaultLoader());
}
