namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to update a resource.
/// </summary>
public class UpdateRequestHandler<TKey, TUpdate, TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null, 
    string? clientName = null) 
    : PutRequestHandler<TKey, TUpdate, TResult>(httpClientFactory, path, httpHandler, clientName)
    where TUpdate : IHasId<TKey>
{

    /// <summary>
    /// Executes the request handler, obtaining the ID from the <see cref="IHasId{TId}"/> property.
    /// </summary>
    public async Task<TResult> ExecuteAsync(TUpdate update, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(update.Id, update, cancellationToken);
    }
}
