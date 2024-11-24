using Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

namespace Ocluse.LiquidSnow.Http.Client;

///<inheritdoc cref="ICrudRequestBuilder{TCreate, TUpdate, TList, TModel, TSummary}"/>
public class CrudRequestBuilder<TKey, TCreate, TUpdate, TList, TModel, TSummary>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null,
    string? clientName = null)
    : ICrudRequestBuilder<TKey, TCreate, TUpdate, TList, TModel, TSummary> 
    where TUpdate : IHasId<TKey>
{
    private readonly CreateRequestHandler<TCreate, TModel> _createHandler = new(httpClientFactory, path, httpHandler, clientName);
    private readonly UpdateRequestHandler<TKey, TUpdate, TModel> _updateHandler = new(httpClientFactory, path, httpHandler, clientName);
    private readonly ReadRequestHandler<TKey, TModel> _readHandler = new(httpClientFactory, path, httpHandler, clientName);
    private readonly DeleteRequestHandler<TKey> _deleteHandler = new(httpClientFactory, path, httpHandler, clientName);
    private readonly ListRequestHandler<TList, TSummary> _listHandler = new(httpClientFactory, path, httpHandler, clientName);

    ///<inheritdoc/>
    public string Path { get; } = path;

    /// <summary>
    /// The factory used to create HTTP clients.
    /// </summary>
    protected ISnowHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

    /// <summary>
    /// The handler used to handle requests and responses.
    /// </summary>
    protected IHttpHandler? HttpHandler { get; } = httpHandler;

    /// <summary>
    /// The name of the client to use when creating HTTP clients.
    /// </summary>
    protected string? ClientName { get; } = clientName;

    ///<inheritdoc/>
    public async Task<TModel> CreateAsync(TCreate create, CancellationToken cancellationToken = default)
    {
        return await _createHandler.ExecuteAsync(create, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<TModel> UpdateAsync(TUpdate update, CancellationToken cancellationToken = default)
    {
        return await _updateHandler.ExecuteAsync(update, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<TModel> ReadAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _readHandler.ExecuteAsync(id, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        await _deleteHandler.ExecuteAsync(id, cancellationToken);
    }

    ///<inheritdoc/>
    public async Task<QueryResult<TSummary>> ListAsync(TList? list, CancellationToken cancellationToken = default)
    {
        return await _listHandler.ExecuteAsync(list, cancellationToken);
    }
}

///<inheritdoc/>
public class CrudRequestBuilder<TKey, TCreate, TUpdate, TList, TModel>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, IHttpHandler? httpHandler = null,
    string? clientName = null)
    : CrudRequestBuilder<TKey, TCreate, TUpdate, TList, TModel, TModel>(httpClientFactory, path, httpHandler, clientName),
    ICrudRequestBuilder<TKey, TCreate, TUpdate, TList, TModel>
    where TUpdate : IHasId<TKey>
{
}
