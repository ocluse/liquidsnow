namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler used to send a put request with a specified Id and content body.
    /// </summary>
    public class PutRequestHandler<TKey, TContent, TResult>
        : ContentWithIdRequestHandler<TKey, TContent, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PutRequestHandler{TKey, TContent,TResult}"/> class
        /// </summary>
        public PutRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
            : base(HttpMethod.Put, httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
