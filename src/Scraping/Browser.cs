using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Ocluse.LiquidSnow.Extensions;
using System.Net;
using System.Text;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Simulates a browser for sending HTTP requests with appropriate headers and cookie management.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Browser"/> class.
/// </remarks>
/// <param name="httpClient">The HttpClient instance to use for sending requests. It's recommended this HttpClient is NOT configured to handle cookies automatically (e.g., its handler's UseCookies=false) as this class manages cookies manually.</param>
/// <param name="browserOptions">The options to use for the browser</param>
public class Browser(HttpClient httpClient, IOptions<BrowserOptions> browserOptions)
{
    private readonly CookieContainer _cookieContainer = new();
    private Uri? _lastSuccessfulNavigationUri;

    /// <summary>
    /// Gets or sets the last successfully navigated URL.
    /// </summary>
    public Uri? LastSuccessfulNavigationUri => _lastSuccessfulNavigationUri;

    /// <summary>
    /// Applies the necessary headers to the HttpRequestMessage to simulate browser behavior.
    /// </summary>
    /// <param name="requestMessage">The request message to modify</param>
    /// <param name="requestOptions">The options determining how the request message will be modified.</param>
    public void ApplyRequestHeaders(HttpRequestMessage requestMessage, RequestOptions requestOptions)
    {
        if (requestMessage.RequestUri == null)
        {
            throw new ArgumentException("RequestUri must be set on the HttpRequestMessage before applying headers.", nameof(requestMessage));
        }

        // --- Prepare Headers ---
        // Clear potentially conflicting headers that we will manage.
        // This ensures our headers take precedence over any defaults on the HttpRequestMessage.
        requestMessage.Headers.Remove("User-Agent");
        requestMessage.Headers.Remove("Accept");
        requestMessage.Headers.Remove("Accept-Language");
        requestMessage.Headers.Remove("Referer");
        requestMessage.Headers.Remove("Origin");
        requestMessage.Headers.Remove("Cookie");
        requestMessage.Headers.Remove("X-Requested-With");
        requestMessage.Headers.Remove("Sec-Fetch-Site");
        requestMessage.Headers.Remove("Sec-Fetch-Mode");
        requestMessage.Headers.Remove("Sec-Fetch-Dest");
        requestMessage.Headers.Remove("Sec-Fetch-User");
        requestMessage.Headers.Remove("Upgrade-Insecure-Requests");


        // 1. User-Agent
        requestMessage.Headers.TryAddWithoutValidation("User-Agent", browserOptions.Value.UserAgent);

        // 2. Accept-Language
        if (browserOptions.Value.AcceptLanguage.IsNotEmpty())
        {
            requestMessage.Headers.TryAddWithoutValidation("Accept-Language", browserOptions.Value.AcceptLanguage);
        }

        // 3. Determine Effective Referrer and Origin
        Uri? effectiveReferrerUrl = requestOptions.ReferrerUrl ?? _lastSuccessfulNavigationUri;
        string? originHeaderValue = null;

        if (effectiveReferrerUrl != null)
        {
            requestMessage.Headers.TryAddWithoutValidation("Referer", effectiveReferrerUrl.AbsoluteUri);
            originHeaderValue = requestOptions.OverrideOrigin ?? $"{effectiveReferrerUrl.Scheme}://{effectiveReferrerUrl.Host}";
        }

        // 4. Sec-Fetch-Site
        string secFetchSiteValue = "none";
        if (effectiveReferrerUrl != null)
        {
            if (AreSameOrigin(requestMessage.RequestUri, effectiveReferrerUrl))
            {
                secFetchSiteValue = "same-origin";
            }
            else
            {
                // Simplified: "cross-site". A more accurate check would involve eTLD+1 for "same-site".
                secFetchSiteValue = "cross-site";
            }
        }
        requestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Site", secFetchSiteValue);

        // 5. Set headers based on RequestType
        string? secFetchModeValue = null;
        string? secFetchDestValue = null;
        string? acceptHeaderValue = null;

        switch (requestOptions.Type)
        {
            case BrowserRequestType.Navigation:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderNavigationAndIframe;
                secFetchModeValue = "navigate";
                secFetchDestValue = "document";
                if (requestOptions.IsUserInitiated)
                {
                    requestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                }
                if (requestMessage.RequestUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                    (requestMessage.Method == HttpMethod.Get || requestMessage.Method == HttpMethod.Head))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                }
                break;

            case BrowserRequestType.Xhr:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderXhr;
                // For XHR, Sec-Fetch-Mode is 'cors' if cross-site, 'same-origin' if same-origin.
                secFetchModeValue = secFetchSiteValue == "cross-site" ? "cors" : "same-origin";
                secFetchDestValue = "empty";
                requestMessage.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                // Origin is often sent for XHR, especially if cross-site or POST/PUT etc.
                if (!string.IsNullOrEmpty(originHeaderValue) && (secFetchSiteValue == "cross-site" || requestMessage.Method != HttpMethod.Get))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Origin", originHeaderValue);
                }
                break;

            case BrowserRequestType.Iframe:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderNavigationAndIframe;
                secFetchModeValue = "navigate"; // Or "nested-navigate"
                secFetchDestValue = "iframe";
                if (!string.IsNullOrEmpty(originHeaderValue) && secFetchSiteValue == "cross-site") // Origin for cross-site iframes
                {
                    requestMessage.Headers.TryAddWithoutValidation("Origin", originHeaderValue);
                }
                break;

            case BrowserRequestType.Script:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderScript;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "script";
                break;

            case BrowserRequestType.Style:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderStyle;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "style";
                break;

            case BrowserRequestType.Image:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderImage;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "image";
                break;

            case BrowserRequestType.Font:
                acceptHeaderValue = browserOptions.Value.AcceptHeaderFont;
                secFetchModeValue = "no-cors"; // Often 'cors' if font is from different origin and needs CORS
                secFetchDestValue = "font";
                // Origin for cross-origin font requests if they require CORS
                if (!string.IsNullOrEmpty(originHeaderValue) && secFetchSiteValue == "cross-site")
                {
                    requestMessage.Headers.TryAddWithoutValidation("Origin", originHeaderValue);
                }
                break;

            case BrowserRequestType.Other:
                acceptHeaderValue = "*/*";
                secFetchModeValue = "no-cors";
                secFetchDestValue = "empty";
                break;
        }

        if (acceptHeaderValue != null) requestMessage.Headers.TryAddWithoutValidation("Accept", acceptHeaderValue);
        if (secFetchModeValue != null) requestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", secFetchModeValue);
        if (secFetchDestValue != null) requestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", secFetchDestValue);

        // 6. Cookie Management
        var cookieBuilder = new StringBuilder();
        string mainCookies = _cookieContainer.GetCookieHeader(requestMessage.RequestUri);
        if (!string.IsNullOrWhiteSpace(mainCookies))
        {
            cookieBuilder.Append(mainCookies);
        }

        if (requestOptions.AdditionalCookies != null)
        {
            foreach (var additionalCookie in requestOptions.AdditionalCookies)
            {
                if (cookieBuilder.Length > 0 && !mainCookies.EndsWith(';')) // Ensure separator if needed
                {
                    // GetCookieHeader might or might not end with a semicolon, be careful
                    if (cookieBuilder.ToString().Split(';').All(c => !c.TrimStart().StartsWith(additionalCookie.Name + "=")))
                    {
                        cookieBuilder.Append("; ");
                    }
                    else
                    {
                        if (cookieBuilder.Length > 0) cookieBuilder.Append("; ");
                    }
                }
                else if (cookieBuilder.Length > 0 && mainCookies.EndsWith(';'))
                {
                    // mainCookies already ends with a separator
                }
                else if (cookieBuilder.Length > 0)
                { // No main cookies, but previous additional cookie
                    cookieBuilder.Append("; ");
                }
                cookieBuilder.Append(additionalCookie.Name).Append('=').Append(additionalCookie.Value);
            }
        }

        if (cookieBuilder.Length > 0)
        {
            requestMessage.Headers.TryAddWithoutValidation("Cookie", cookieBuilder.ToString());
        }

        // 7. Apply any custom headers from options
        if (requestOptions.CustomHeaders != null)
        {
            foreach (var customHeader in requestOptions.CustomHeaders)
            {
                requestMessage.Headers.Remove(customHeader.Key); // Remove if exists
                requestMessage.Headers.TryAddWithoutValidation(customHeader.Key, customHeader.Value);
            }
        }
    }

    ///<inheritdoc cref="SendAsync(HttpRequestMessage, RequestOptions, CancellationToken)"/>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        return await SendAsync(requestMessage, new RequestOptions(), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request asynchronously, simulating browser behavior.
    /// </summary>
    /// <param name="requestMessage">The HttpRequestMessage to send. Headers on this message may be modified.</param>
    /// <param name="requestOptions">Options for the request, like type, referrer, and additional cookies.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The HttpResponseMessage from the server.</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        if (requestMessage.RequestUri == null)
        {
            throw new ArgumentException("RequestUri must be set on the HttpRequestMessage before applying headers.", nameof(requestMessage));
        }

        // --- Apply Headers ---
        ApplyRequestHeaders(requestMessage, requestOptions);

        // --- Send Request ---
        HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

        // --- Process Response ---
        // 1. Update Cookies
        if (responseMessage.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? setCookieHeaders))
        {
            foreach (string setCookieHeader in setCookieHeaders)
            {
                try
                {
                    _cookieContainer.SetCookies(requestMessage.RequestUri, setCookieHeader);
                }
                catch (CookieException ex)
                {
                    // Log or handle cookie parsing errors if necessary
                    System.Diagnostics.Debug.WriteLine($"Cookie parsing error for URI {requestMessage.RequestUri}: {ex.Message} on header: {setCookieHeader}");
                }
            }
        }

        // 2. Update Last Successful Navigation URL
        if (requestOptions.Type == BrowserRequestType.Navigation && responseMessage.IsSuccessStatusCode)
        {
            _lastSuccessfulNavigationUri = requestMessage.RequestUri;

            // Handle redirects for navigation
            if (responseMessage.StatusCode == HttpStatusCode.Redirect ||
                responseMessage.StatusCode == HttpStatusCode.MovedPermanently ||
                responseMessage.StatusCode == HttpStatusCode.Found || // HttpStatusCode.Found is 302
                responseMessage.StatusCode == HttpStatusCode.SeeOther ||
                responseMessage.StatusCode == HttpStatusCode.TemporaryRedirect ||
                (int)responseMessage.StatusCode == 308) // PermanentRedirect
            {
                if (responseMessage.Headers.Location != null)
                {
                    Uri redirectUri = responseMessage.Headers.Location;
                    if (!redirectUri.IsAbsoluteUri)
                    {
                        redirectUri = new Uri(_lastSuccessfulNavigationUri, redirectUri);
                    }
                    _lastSuccessfulNavigationUri = redirectUri; // Update to the final location after redirect
                }
            }
        }

        return responseMessage;
    }

    /// <summary>
    /// Sends a GET request to the specified URL and returns a Document object representing the HTML content of the response.
    /// </summary>
    public async Task<IDocument> GetDocumentAsync(Uri requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, requestUri);

        using HttpResponseMessage responseMessage = await SendAsync(requestMessage, requestOptions, cancellationToken);

        string html = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        return await GetFromHtmlAsync(responseMessage.RequestMessage?.RequestUri ?? requestUri, html, cancellationToken);

    }

    /// <summary>
    /// Returns a Document object representing the HTML content of the response.
    /// </summary>
    public async Task<IDocument> GetFromHtmlAsync(Uri address, string html, CancellationToken cancellationToken = default)
    {
        return await browserOptions.Value.BrowsingContext.OpenAsync(req =>
        {
            req.Address(address);
            req.Content(html);
        }, cancellationToken);
    }

    /// <summary>
    /// Helper method to determine if two URIs are of the same origin (scheme, host, port).
    /// </summary>
    private static bool AreSameOrigin(Uri? uri1, Uri? uri2)
    {
        if (uri1 == null || uri2 == null) return false;
        return uri1.Scheme.Equals(uri2.Scheme, StringComparison.OrdinalIgnoreCase) &&
               uri1.Host.Equals(uri2.Host, StringComparison.OrdinalIgnoreCase) &&
               uri1.Port == uri2.Port;
    }

    /// <summary>
    /// Gets all cookies currently stored in the CookieContainer.
    /// </summary>
    /// <returns>A list of all cookies.</returns>
    public List<Cookie> GetCookies(Uri uri)
    {
        return [.. _cookieContainer.GetCookies(uri).Cast<Cookie>()];
    }
}
