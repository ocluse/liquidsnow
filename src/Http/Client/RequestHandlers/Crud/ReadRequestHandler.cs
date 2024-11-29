namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to get a resource by id.
/// </summary>
public class ReadRequestHandler<TKey, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : GetByIdRequestHandler<TKey, TResult>(httpClientFactory, path, httpHandler, clientName)
{
}
