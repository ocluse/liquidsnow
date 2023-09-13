namespace Ocluse.LiquidSnow.Http.Client.RequestHandler
{
    /// <summary>
    /// A handler used to send a post request with content body.
    /// </summary>
    public class PostRequestHandler<TContent, TResult> 
        : ContentRequestHandler<TContent, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PostRequestHandler{TContent,TResult}"/> class
        /// </summary>
        public PostRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(HttpMethod.Post, httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
