namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to create a resource.
/// </summary>
public class CreateRequestHandler<TCreate, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : PostRequestHandler<TCreate, TResult>(httpClientFactory, path, httpHandler, clientName)
{
}
