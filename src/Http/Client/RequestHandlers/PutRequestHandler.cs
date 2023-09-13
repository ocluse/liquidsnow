namespace Ocluse.LiquidSnow.Http.Client.RequestHandler
{
    /// <summary>
    /// A handler used to send a put request with a specified Id and content body.
    /// </summary>
    public class PutRequestHandler<TContent, TResult> 
        : ContentWithIdRequestHandler<TContent, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PutRequestHandler{TContent,TResult}"/> class
        /// </summary>
        public PutRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(HttpMethod.Put, httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
