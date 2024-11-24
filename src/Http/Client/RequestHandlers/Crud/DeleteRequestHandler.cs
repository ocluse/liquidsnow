using System.Reactive;

namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to delete a resource.
/// </summary>
public class DeleteRequestHandler<TKey>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : DeleteByIdRequestHandler<TKey, Unit>(httpClientFactory, path, httpHandler, clientName)
{
}
