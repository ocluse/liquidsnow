using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Extensions for the <see cref="Browser"/> class to simplify various requests.
/// </summary>
public static class BrowserExtensions
{
    
    ///<inheritdoc cref="GetFromJsonAsync{T}(Browser, Uri, RequestOptions, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<T?> GetFromJsonAsync<T>(this Browser browser, Uri requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromJsonAsync<T>(requestUri, requestOptions, null, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request to the specified URI and deserializes the response content into an object of type T.
    /// </summary>
    public static async Task<T?> GetFromJsonAsync<T>(this Browser browser, Uri requestUri, RequestOptions requestOptions, JsonSerializerOptions? jsonOptions, CancellationToken cancellationToken = default)
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
}
