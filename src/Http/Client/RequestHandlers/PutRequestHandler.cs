namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a put request with a specified Id and content body.
/// </summary>
public class PutRequestHandler<TKey, TContent, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null, 
    string? clientName = null)
    : ContentWithIdRequestHandler<TKey, TContent, TResult>(HttpMethod.Put, httpClientFactory, path, httpHandler, clientName)
{
}
