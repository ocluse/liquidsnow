namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A request handler that sends a HTTP request with a content and an id based path
    /// </summary>
    public class ContentWithIdRequestHandler<TKey, TContent, TResult> : RequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ContentWithIdRequestHandler{TKey, TContent,TResult}"/> class
        /// </summary>
        public ContentWithIdRequestHandler(
            HttpMethod httpMethod,
            ISnowHttpClientFactory httpClientFactory,
            string path,
            IHttpHandler? httpHandler = null,
            string? clientName = null)
            : base(httpClientFactory, path, httpHandler, clientName)
        {
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// The HTTP method to use
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Gets the path to use for the request.
        /// </summary>
        /// <remarks>
        /// The result returned by this method will be transformed by <see cref="GetTransformedUrlPath"/> before sending the request.
        /// </remarks>
        protected virtual string GetUrlPath(TKey id, TContent content)
        {
            return $"{Path}/{GetPathSegmentFromId(id)}";
        }

        /// <summary>
        /// Gets the transformed final path to use for the request.
        /// </summary>
        protected virtual string GetTransformedUrlPath(TKey id, TContent content)
        {
            string path = GetUrlPath(id, content);

            return TransformUrlPath(path);
        }

        /// <summary>
        /// Sends a request with the given content and id, returning the result.
        /// </summary>
        public async Task<TResult> ExecuteAsync(TKey id, TContent content, CancellationToken cancellationToken = default)
        {
            string path = GetTransformedUrlPath(id, content);
            HttpContent httpContent = await GetContent(content, cancellationToken);

            using HttpRequestMessage request = new(HttpMethod, path);

            request.Content = httpContent;

            return await SendAsync(request, cancellationToken);
        }
    }
}
