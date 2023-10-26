using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud
{
    /// <summary>
    /// A handler user to update a resource
    /// </summary>
    public class UpdateRequestHandler<TKey, TUpdate, TResult> : PutRequestHandler<TKey, TUpdate, TResult>
        where TUpdate : IKeyCommand<TKey, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UpdateRequestHandler{TKey, TUpdate,TResult}"/> class
        /// </summary>
        public UpdateRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null) 
            : base(httpClientFactory, path, httpHandler, clientName)
        {
        }

        /// <summary>
        /// Executes the request handler, obtaining the ID from the <see cref="IKeyCommand{TKey, TResult}.Id"/> property
        /// </summary>
        public async Task<TResult> ExecuteAsync(TUpdate update, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(update.Id, update, cancellationToken);
        }
    }
}
