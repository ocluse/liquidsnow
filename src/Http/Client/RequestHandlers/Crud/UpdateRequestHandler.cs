using Ocluse.LiquidSnow.Http.Client.RequestHandler;
using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud
{
    /// <summary>
    /// A handler user to update a resource
    /// </summary>
    public class UpdateRequestHandler<TUpdate, TResult> : PutRequestHandler<TUpdate, TResult>
        where TUpdate : IKeyCommand<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UpdateRequestHandler{TUpdate,TResult}"/> class
        /// </summary>
        public UpdateRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
        }

        /// <summary>
        /// Executes the request handler, obtaining the ID from the <see cref="IKeyCommand{TResult}.Id"/> property
        /// </summary>
        public async Task<TResult> ExecuteAsync(TUpdate update, CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(update.Id, update, cancellationToken);
        }
    }
}
