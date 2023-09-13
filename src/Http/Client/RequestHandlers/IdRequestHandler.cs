namespace Ocluse.LiquidSnow.Http.Client.RequestHandler
{
    /// <summary>
    /// A request handler used to send a request with an id based path
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class IdRequestHandler<TResult> : RequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IdRequestHandler{TResult}"/> class
        /// </summary>
        public IdRequestHandler(HttpMethod httpMethod, ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// The http method to use for the request
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Gets the url path for the request
        /// </summary>
        /// <remarks>
        /// The path returned by this method will be transformed by the <see cref="GetTransformedUrlPath"/> method before making the request.
        /// </remarks>
        protected virtual string GetUrlPath(string id)
        {
            return $"{Path}/{id}";
        }

        /// <summary>
        /// The method used to make the final transformation to the url path before making the request
        /// </summary>
        protected virtual string GetTransformedUrlPath(string id)
        {
            string path = GetUrlPath(id);

            return TransformUrlPath(path);
        }

        /// <summary>
        /// Sends a request with the given id, returning the result.
        /// </summary>
        public async Task<TResult> ExecuteAsync(string id, CancellationToken cancellationToken = default)
        {
            string path = GetTransformedUrlPath(id);

            using HttpRequestMessage request = new(HttpMethod.Get, path);
            return await SendAsync(request, cancellationToken);
        }
    }
}
