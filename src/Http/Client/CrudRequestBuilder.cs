using Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;
using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client
{
    ///<inheritdoc cref="ICrudRequestBuilder{TCreate, TUpdate, TList, TModel, TSummary}"/>
    public class CrudRequestBuilder<TCreate, TUpdate, TList, TModel, TSummary> 
        : ICrudRequestBuilder<TCreate, TUpdate, TList, TModel, TSummary> where TUpdate : IKeyCommand<TModel>
    {
        private readonly CreateRequestHandler<TCreate, TModel> _createHandler;
        private readonly UpdateRequestHandler<TUpdate, TModel> _updateHandler;
        private readonly ReadRequestHandler<TModel> _readHandler;
        private readonly DeleteRequestHandler _deleteHandler;
        private readonly ListRequestHandler<TList, TSummary> _listHandler;

        /// <summary>
        /// Creates the request builder for the specified REST path.
        /// </summary>
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null)
        {
            _createHandler = new(httpClientFactory, path, clientName, httpHandler);
            _updateHandler = new(httpClientFactory, path, clientName, httpHandler);
            _readHandler = new(httpClientFactory, path, clientName, httpHandler);
            _deleteHandler = new(httpClientFactory, path, clientName, httpHandler);
            _listHandler = new(httpClientFactory, path, clientName, httpHandler);
        }

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
        public async Task<TModel> ReadAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _readHandler.ExecuteAsync(id, cancellationToken);
        }

        ///<inheritdoc/>
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
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
    public class CrudRequestBuilder<TCreate, TUpdate, TList, TModel> 
        : CrudRequestBuilder<TCreate, TUpdate, TList, TModel, TModel>,
        ICrudRequestBuilder<TCreate, TUpdate, TList, TModel>
        where TUpdate : IKeyCommand<TModel>
    {
        /// <summary>
        /// Creates the request builder for the specified REST path.
        /// </summary>
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
