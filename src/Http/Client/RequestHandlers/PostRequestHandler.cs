namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a post request with content body.
/// </summary>
public class PostRequestHandler<TContent, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null, 
    string? clientName = null)
    : ContentRequestHandler<TContent, TResult>(HttpMethod.Post, httpClientFactory, path, httpHandler, clientName)
{
}
