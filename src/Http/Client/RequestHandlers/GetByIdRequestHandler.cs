namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler used to send a get request to a resource by id
    /// </summary>
    public class GetByIdRequestHandler<TResult> : IdRequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetByIdRequestHandler{TResult}"/> class
        /// </summary>
        public GetByIdRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
            : base(HttpMethod.Get, httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
