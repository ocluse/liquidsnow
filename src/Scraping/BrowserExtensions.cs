using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Ocluse.LiquidSnow.Scraping;

internal static class BrowserExtensions
{
    public static async Task<T?> GetFromJsonAsync<T>(this Browser browser, Uri requestUri, RequestOptions requestOptions, CancellationToken cancellationToken = default)
    {
        return await browser.GetFromJsonAsync<T>(requestUri, requestOptions, null, cancellationToken);
    }

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
