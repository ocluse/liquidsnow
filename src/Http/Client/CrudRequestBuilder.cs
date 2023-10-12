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
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
        {
            _createHandler = new(httpClientFactory, path, httpHandler, clientName);
            _updateHandler = new(httpClientFactory, path, httpHandler, clientName);
            _readHandler = new(httpClientFactory, path, httpHandler, clientName);
            _deleteHandler = new(httpClientFactory, path, httpHandler, clientName);
            _listHandler = new(httpClientFactory, path, httpHandler, clientName);

            Path = path;
            HttpClientFactory = httpClientFactory;
            HttpHandler = httpHandler;
            ClientName = clientName;
        }

        ///<inheritdoc/>
        public string Path { get; }

        /// <summary>
        /// The factory used to create HTTP clients.
        /// </summary>
        protected ISnowHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// The handler used to handle requests and responses.
        /// </summary>
        protected IHttpHandler? HttpHandler { get; }

        /// <summary>
        /// The name of the client to use when creating HTTP clients.
        /// </summary>
        protected string? ClientName { get; }

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
        public CrudRequestBuilder(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
            : base(httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
