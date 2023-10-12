namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler used send a delete request by query string parameters
    /// </summary>
    public class DeleteByQueryRequestHandler<TQuery, TResult> : QueryRequestHandler<TQuery, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DeleteByQueryRequestHandler{TQuery,TResult}"/> class
        /// </summary>
        public DeleteByQueryRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null) 
            : base(HttpMethod.Delete, httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
