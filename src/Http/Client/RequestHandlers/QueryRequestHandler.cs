namespace Ocluse.LiquidSnow.Http.Client.RequestHandler
{
    /// <summary>
    /// A handler that sends http request to a server by query string parameters.
    /// </summary>
    public class QueryRequestHandler<TQuery, TResult> : RequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="QueryRequestHandler{TQuery,TResult}"/> class
        /// </summary>
        public QueryRequestHandler(HttpMethod httpMethod, ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// The http method to use when sending the request
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Converts the query object into a path with a query string.
        /// </summary>
        protected virtual string GetUrlPath(TQuery? query)
        {
            if (query == null)
            {
                return Path;
            }
            return $"{Path}?{GetQueryString(query)}";
        }
        /// <summary>
        /// The final transformation applied to the url path before sending the request.
        /// </summary>
        protected virtual string GetTransformedUrlPath(TQuery? query)
        {
            string path = GetUrlPath(query);

            return TransformUrlPath(path);
        }

        /// <summary>
        /// Sends the request with the given query string parameters, returning the result.
        /// </summary>
        public async Task<TResult> ExecuteAsync(TQuery? query, CancellationToken cancellationToken = default)
        {
            string path = GetTransformedUrlPath(query);
            using HttpRequestMessage request = new(HttpMethod, path);
            return await SendAsync(request, cancellationToken);
        }
    }
}
