namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler used to send a delete request to a resource by id
    /// </summary>
    public class DeleteByIdRequestHandler<TResult> : IdRequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DeleteByIdRequestHandler{TResult}"/> class
        /// </summary>
        public DeleteByIdRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string ? clientName = null)
            : base(HttpMethod.Delete, httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
