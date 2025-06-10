using System.Net;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Options for a simulated browser request.
/// </summary>
public class RequestOptions
{
    /// <summary>
    /// The type of the request (e.g., Navigation, Xhr). Defaults to Navigation.
    /// </summary>
    public BrowserRequestType Type { get; init; } = BrowserRequestType.Navigation;

    /// <summary>
    /// The Referrer URL for the request. If null, the last successfully navigated URL by this Browser instance might be used.
    /// </summary>
    public Uri? ReferrerUrl { get; init; }

    /// <summary>
    /// Additional cookies to be sent with this specific request.
    /// These cookies are added to the Cookie header along with those from the main CookieContainer.
    /// </summary>
    public IEnumerable<Cookie>? AdditionalCookies { get; init; }

    /// <summary>
    /// Indicates if the request is user-initiated. Used for Sec-Fetch-User header. Defaults to true.
    /// </summary>
    public bool IsUserInitiated { get; init; } = true;

    /// <summary>
    /// Allows overriding the Origin header. If null, Origin is derived from ReferrerUrl.
    /// </summary>
    public string? OverrideOrigin { get; init; }

    /// <summary>
    /// Optional custom headers to add or override for this specific request.
    /// Key: Header name, Value: Header value.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>>? CustomHeaders { get; init; }

    /// <summary>
    /// An implicit operator to convert a BrowserRequestType to RequestOptions.
    /// </summary>
    public static implicit operator RequestOptions(BrowserRequestType type)
    {
        return new RequestOptions { Type = type };
    }
}
