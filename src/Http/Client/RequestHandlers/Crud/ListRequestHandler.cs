namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to get a list of resources.
/// </summary>
public class ListRequestHandler<TQuery, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : GetByQueryRequestHandler<TQuery, QueryResult<TResult>>(httpClientFactory, path, httpHandler, clientName)
{
}
