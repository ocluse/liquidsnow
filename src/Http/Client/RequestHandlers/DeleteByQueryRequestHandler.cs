namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used send a delete request by query string parameters.
/// </summary>
public class DeleteByQueryRequestHandler<TQuery, TResult>(
    ISnowHttpClientFactory httpClientFactory,
    string path,
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : QueryRequestHandler<TQuery, TResult>(HttpMethod.Delete, httpClientFactory, path, httpHandler, clientName)
{
}
