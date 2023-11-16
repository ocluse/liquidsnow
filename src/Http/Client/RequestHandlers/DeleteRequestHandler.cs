namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler user to delete a resource
    /// </summary>
    public class DeleteRequestHandler<TResult> : RequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DeleteRequestHandler{TResult}"/> class
        /// </summary>
        public DeleteRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
            : base(httpClientFactory, path, httpHandler, clientName)
        {
        }

        /// <summary>
        /// Gets the url path for the request
        /// </summary>
        /// <remarks>
        /// The path returned by this method will be transformed by the <see cref="GetTransformedUrlPath"/> method before making the request.
        /// </remarks>
        protected virtual string GetUrlPath()
        {
            return Path;
        }

        /// <summary>
        /// The method used to make the final transformation to the url path before making the request
        /// </summary>
        protected virtual string GetTransformedUrlPath()
        {
            string path = GetUrlPath();

            return TransformUrlPath(path);
        }

        /// <summary>
        /// Sends a request with the given id, returning the result.
        /// </summary>
        public async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            string path = GetTransformedUrlPath();

            using HttpRequestMessage request = new(HttpMethod.Delete, path);
            return await SendAsync(request, cancellationToken);
        }
    }
}
