namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a get request to a resource by id
/// </summary>
public class GetByIdRequestHandler<TKey, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path,
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : IdRequestHandler<TKey, TResult>(HttpMethod.Get, httpClientFactory, path, httpHandler, clientName)
{
}
