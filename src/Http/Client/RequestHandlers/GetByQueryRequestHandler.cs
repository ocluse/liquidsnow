namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a get request to a resource by query
/// </summary>
public class GetByQueryRequestHandler<TQuery, TResult>(
    ISnowHttpClientFactory httpClientFactory,
    string path, IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : QueryRequestHandler<TQuery, TResult>(HttpMethod.Get, httpClientFactory, path, httpHandler, clientName)
{
}
