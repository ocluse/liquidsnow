namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a delete request to a resource by id.
/// </summary>
public class DeleteByIdRequestHandler<TKey, TResult>(
    ISnowHttpClientFactory httpClientFactory,
    string path,
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : IdRequestHandler<TKey, TResult>(HttpMethod.Delete, httpClientFactory, path, httpHandler, clientName)
{
}
