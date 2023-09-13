namespace Ocluse.LiquidSnow.Http.Client.RequestHandler
{
    /// <summary>
    /// A handler used to send a get request to a resource by id
    /// </summary>
    public class GetByIdRequestHandler<TResult> : IdRequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetByIdRequestHandler{TResult}"/> class
        /// </summary>
        public GetByIdRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(HttpMethod.Get, httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
