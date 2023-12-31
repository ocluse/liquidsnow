﻿namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers
{
    /// <summary>
    /// A handler used to send a get request to a resource by query
    /// </summary>
    public class GetByQueryRequestHandler<TQuery, TResult> : QueryRequestHandler<TQuery, TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GetByQueryRequestHandler{TQuery,TResult}"/> class
        /// </summary>
        public GetByQueryRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string ? clientName = null)
            : base(HttpMethod.Get, httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
