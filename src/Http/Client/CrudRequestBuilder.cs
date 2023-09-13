using Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;
using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// Provides utility methods for making CRUD requests in a RESTful manner
    /// </summary>
    /// <typeparam name="TCreate">The request sent to create a resource</typeparam>
    /// <typeparam name="TUpdate">The request sent to update a resource</typeparam>
    /// <typeparam name="TList">The request sent as a query to fetch a list of items</typeparam>
    /// <typeparam name="TModel">The main model type of the resource received from the server</typeparam>
    /// <typeparam name="TSummary">The model type received from the server for LIST/QUERY operations</typeparam>
    public class CrudRequestBuilder<TCreate, TUpdate, TList, TModel, TSummary>
        where TUpdate : IKeyCommand<TModel>
    {
        private readonly CreateRequestHandler<TCreate, TModel> _createHandler;
        private readonly UpdateRequestHandler<TUpdate, TModel> _updateHandler;
        private readonly ReadRequestHandler<TModel> _readHandler;
        private readonly DeleteRequestHandler _deleteHandler;
        private readonly ListRequestHandler<TList, TSummary> _listHandler;

        /// <summary>
        /// Creates the request builder to send requests to the specified path in a RESTful manner
        /// </summary>
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null)
        {
            _createHandler = new(httpClientFactory, path, clientName, httpHandler);
            _updateHandler = new(httpClientFactory, path, clientName, httpHandler);
            _readHandler = new(httpClientFactory, path, clientName, httpHandler);
            _deleteHandler = new(httpClientFactory, path, clientName, httpHandler);
            _listHandler = new(httpClientFactory, path, clientName, httpHandler);
        }

        /// <summary>
        /// Sends a request to create a resource
        /// </summary>
        public async Task<TModel> CreateAsync(TCreate create, CancellationToken cancellationToken = default)
        {
            return await _createHandler.ExecuteAsync(create, cancellationToken);
        }

        /// <summary>
        /// Sends a request to update a resource
        /// </summary>
        public async Task<TModel> UpdateAsync(TUpdate update, CancellationToken cancellationToken = default)
        {
            return await _updateHandler.ExecuteAsync(update, cancellationToken);
        }

        /// <summary>
        /// Sends a request to read a resource
        /// </summary>
        public async Task<TModel> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _readHandler.ExecuteAsync(id, cancellationToken);
        }

        /// <summary>
        /// Sends a request to delete a resource
        /// </summary>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _deleteHandler.ExecuteAsync(id, cancellationToken);
        }

        /// <summary>
        /// Send a request to query a list of resources
        /// </summary>
        public async Task<QueryResult<TSummary>> ListAsync(TList? list, CancellationToken cancellationToken = default)
        {
            return await _listHandler.ExecuteAsync(list, cancellationToken);
        }
    }

    ///<inheritdoc/>
    public class CrudRequestBuilder<TCreate, TUpdate, TList, TModel> : CrudRequestBuilder<TCreate, TUpdate, TList, TModel, TModel>
        where TUpdate : IKeyCommand<TModel>
    {
        /// <summary>
        /// Creates the request builder to send requests to the specified path in a RESTful manner
        /// </summary>
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
