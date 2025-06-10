using AngleSharp.Dom;
using Ocluse.LiquidSnow.Scraping.Internal;
using System.Net;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Simulates a browser for sending HTTP requests with appropriate headers and cookie management.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Browser"/> class.
/// </remarks>
public interface IBrowser
{
    /// <summary>
    /// Gets or sets the current location, that determines the base URL for relative requests.
    /// </summary>
    Uri? Location { get; set; }

    /// <summary>
    /// Applies the necessary headers to the HttpRequestMessage to simulate browser behavior.
    /// </summary>
    /// <param name="requestMessage">The request message to modify</param>
    /// <param name="requestOptions">The options determining how the request message will be modified.</param>
    void ApplyRequestHeaders(HttpRequestMessage requestMessage, RequestOptions requestOptions);

    /// <summary>
    /// Gets all cookies currently stored in the CookieContainer.
    /// </summary>
    /// <returns>A list of all cookies.</returns>
    List<Cookie> GetCookies(Uri uri);

    /// <summary>
    /// Sends a GET request to the specified URL and returns a Document object representing the HTML content of the response.
    /// </summary>
    Task<IDocument> GetDocumentAsync(Uri? requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns a Document object representing the HTML content of the response.
    /// </summary>
    Task<IDocument> GetFromHtmlAsync(Uri address, string html, CancellationToken cancellationToken = default);

    ///<inheritdoc cref="SendAsync(HttpRequestMessage, RequestOptions, CancellationToken)"/>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an HTTP request asynchronously, simulating browser behavior.
    /// </summary>
    /// <param name="requestMessage">The HttpRequestMessage to send. Headers on this message may be modified.</param>
    /// <param name="requestOptions">Options for the request, like type, referrer, and additional cookies.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>The HttpResponseMessage from the server.</returns>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, RequestOptions requestOptions, CancellationToken cancellationToken = default);
}