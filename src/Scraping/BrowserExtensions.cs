using AngleSharp.Dom;
using Ocluse.LiquidSnow.Extensions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Extensions for the <see cref="IBrowser"/> class to simplify various requests.
/// </summary>
public static class BrowserExtensions
{

    ///<inheritdoc cref="GetFromJsonAsync{T}(IBrowser, Uri, RequestOptions, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<T?> GetFromJsonAsync<T>(this IBrowser browser, Uri? requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromJsonAsync<T>(requestUri, requestOptions, null, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request to the specified URI and deserializes the response content into an object of type T.
    /// </summary>
    public static async Task<T?> GetFromJsonAsync<T>(this IBrowser browser, Uri? requestUri, RequestOptions requestOptions, JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage requestMessage = new(HttpMethod.Get, requestUri);
        using HttpResponseMessage responseMessage = await browser.SendAsync(requestMessage, requestOptions, cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        responseMessage.EnsureSuccessStatusCode();

        return await responseMessage.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
    }


    ///<inheritdoc cref="GetFromJsonAsync{T}(IBrowser, Uri, RequestOptions, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<T?> GetFromJsonAsync<T>(this IBrowser browser, string? requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromJsonAsync<T>(requestUri?.ToUri(), requestOptions, cancellationToken);
    }

    ///<inheritdoc cref="GetFromJsonAsync{T}(IBrowser, Uri, RequestOptions, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<T?> GetFromJsonAsync<T>(this IBrowser browser, string? requestUri, RequestOptions requestOptions, JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromJsonAsync<T>(requestUri?.ToUri(), requestOptions, jsonOptions, cancellationToken);
    }

    ///<inheritdoc cref="IBrowser.GetDocumentAsync(Uri?, RequestOptions, CancellationToken)"/>
    public static async Task<IDocument> GetDocumentAsync(this IBrowser browser, string? requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetDocumentAsync(requestUri?.ToUri(), requestOptions, cancellationToken);
    }

    ///<inheritdoc cref="IBrowser.GetDocumentAsync(Uri?, RequestOptions, CancellationToken)"/>/>
    public static async Task<IDocument> GetFromHtmlAsync(this IBrowser browser, string address, string html, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromHtmlAsync(address.ToUri(), html, cancellationToken);
    }
}
