using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using Ocluse.LiquidSnow.Extensions;
using System.Net;
using System.Text;

namespace Ocluse.LiquidSnow.Scraping.Internal;


internal class Browser(IHttpClientFactory httpClientFactory, BrowserOptions browserOptions) : IBrowser
{
    internal const string HTTPCLIET_NAME = "Ocluse.LiquidSnow.Scraping.BrowserHttpClient";
    private readonly CookieContainer _cookieContainer = new();
    private Uri? _location;

    public Uri? Location { get; set; }

    public void ApplyRequestHeaders(HttpRequestMessage requestMessage, RequestOptions requestOptions)
    {
        if (requestOptions.ReferrerUrl != null && !requestOptions.ReferrerUrl.IsAbsoluteUri)
        {
            throw new ArgumentException("ReferrerUrl must be an absolute URI.", nameof(requestOptions));
        }

        // --- Prepare Headers ---
        // Clear potentially conflicting headers that we will manage.
        // This ensures our headers take precedence over any defaults on the HttpRequestMessage.
        requestMessage.Headers.Remove("User-Agent");
        requestMessage.Headers.Remove("Accept-Language");
        requestMessage.Headers.Remove("Accept");
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
        requestMessage.Headers.TryAddWithoutValidation("User-Agent", browserOptions.UserAgent);

        // 2. Accept-Language
        if (browserOptions.AcceptLanguage.IsNotEmpty())
        {
            requestMessage.Headers.TryAddWithoutValidation("Accept-Language", browserOptions.AcceptLanguage);
        }

        // 3. Determine Effective Referrer and Origin
        Uri? effectiveReferralUrl = requestOptions.ReferrerUrl ?? _location;
        string? originHeaderValue = null;

        if (effectiveReferralUrl != null)
        {
            requestMessage.Headers.TryAddWithoutValidation("Referer", effectiveReferralUrl.AbsoluteUri);
            originHeaderValue = requestOptions.OverrideOrigin ?? $"{effectiveReferralUrl.Scheme}://{effectiveReferralUrl.Host}";
        }

        // 4. Sec-Fetch-Site
        string secFetchSiteValue = "none";
        if (effectiveReferralUrl != null)
        {
            if (AreSameOrigin(effectiveReferralUrl, requestMessage.RequestUri))
            {
                secFetchSiteValue = "same-origin";
            }
            else
            {
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
                acceptHeaderValue = browserOptions.AcceptHeaderNavigationAndIframe;
                secFetchModeValue = "navigate";
                secFetchDestValue = "document";
                if (requestOptions.IsUserInitiated)
                {
                    requestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");
                }

                if (requestMessage.RequestUri?.IsAbsoluteUri == true && requestMessage.RequestUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                    (requestMessage.Method == HttpMethod.Get || requestMessage.Method == HttpMethod.Head))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
                }
                break;

            case BrowserRequestType.Xhr:
                acceptHeaderValue = browserOptions.AcceptHeaderXhr;
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
                acceptHeaderValue = browserOptions.AcceptHeaderNavigationAndIframe;
                secFetchModeValue = "navigate"; // Or "nested-navigate"
                secFetchDestValue = "iframe";
                if (!string.IsNullOrEmpty(originHeaderValue) && secFetchSiteValue == "cross-site") // Origin for cross-site iframes
                {
                    requestMessage.Headers.TryAddWithoutValidation("Origin", originHeaderValue);
                }
                break;

            case BrowserRequestType.Script:
                acceptHeaderValue = browserOptions.AcceptHeaderScript;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "script";
                break;

            case BrowserRequestType.Style:
                acceptHeaderValue = browserOptions.AcceptHeaderStyle;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "style";
                break;

            case BrowserRequestType.Image:
                acceptHeaderValue = browserOptions.AcceptHeaderImage;
                secFetchModeValue = "no-cors";
                secFetchDestValue = "image";
                break;

            case BrowserRequestType.Font:
                acceptHeaderValue = browserOptions.AcceptHeaderFont;
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
        Uri? cookieUri = _location;
        string mainCookies = string.Empty;

        if (requestMessage.RequestUri?.IsAbsoluteUri == true)
        {
            cookieUri = requestMessage.RequestUri;
        }

        if (cookieUri != null && cookieUri.IsAbsoluteUri)
        {
            mainCookies = _cookieContainer.GetCookieHeader(cookieUri);
        }

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

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
    {
        return await SendAsync(requestMessage, new RequestOptions(), cancellationToken);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {

        if (Location == null && requestMessage.RequestUri == null)
        {
            throw new ArgumentException("Either Location or RequestUri must be set before sending a request.", nameof(requestMessage));
        }

        ApplyRequestHeaders(requestMessage, requestOptions);

        HttpClient httpClient = httpClientFactory.CreateClient(HTTPCLIET_NAME);

        if (_location != null)
        {
            httpClient.BaseAddress = _location;
        }

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
                    _cookieContainer.SetCookies(responseMessage.RequestMessage!.RequestUri!, setCookieHeader);
                }
                catch (CookieException ex)
                {
                    // Log or handle cookie parsing errors if necessary
                    System.Diagnostics.Debug.WriteLine($"Cookie parsing error for URI {requestMessage.RequestUri}: {ex.Message} on header: {setCookieHeader}");
                }
            }
        }

        if (requestOptions.Type == BrowserRequestType.Navigation)
        {
            _location = responseMessage.RequestMessage!.RequestUri!;

            if (responseMessage.IsSuccessStatusCode)
            {
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
                            redirectUri = new Uri(_location, redirectUri);
                        }
                        _location = redirectUri; // Update to the final location after redirect
                    }
                }
            }
        }

        return responseMessage;
    }

    public async Task<IDocument> GetDocumentAsync(Uri? requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, requestUri);

        using HttpResponseMessage responseMessage = await SendAsync(requestMessage, requestOptions, cancellationToken);

        string html = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        return await GetFromHtmlAsync(responseMessage.RequestMessage?.RequestUri ?? requestUri, html, cancellationToken);

    }

    public async Task<IDocument> GetFromHtmlAsync(Uri? address, string html, CancellationToken cancellationToken = default)
    {
        return await browserOptions.BrowsingContext.OpenAsync(req =>
        {
            req.Address(address);
            req.Content(html);
        }, cancellationToken);
    }

    private static bool AreSameOrigin(Uri location, Uri? requestUri)
    {
        if (requestUri == null || !requestUri.IsAbsoluteUri)
        {
            return true;
        }

        return location.Scheme.Equals(requestUri.Scheme, StringComparison.OrdinalIgnoreCase) &&
               location.Host.Equals(requestUri.Host, StringComparison.OrdinalIgnoreCase) &&
               location.Port == requestUri.Port;
    }

    public List<Cookie> GetCookies(Uri uri)
    {
        return [.. _cookieContainer.GetCookies(uri).Cast<Cookie>()];
    }
}
