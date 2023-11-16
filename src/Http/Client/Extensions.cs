using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// Adds extension methods to types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds the required plumbing to the service collection to use the Snow clients
        /// </summary>
        public static ISnowClientBuilder AddSnowClients<T>(this IServiceCollection services)
             where T : class, ISnowHttpClientFactory
        {
            services.AddSingleton<ISnowHttpClientFactory, T>();

            return new SnowClientBuilder(services);
        }

        /// <summary>
        /// Returns the handler as the specified type or null if it cannot be cast. If the handler is of type <see cref="IHttpHandlerProvider"/>, the provider is used to get the handler.
        /// </summary>
        public static T? As<T>(this IHttpHandler? handler)
            where T : IHttpHandler
        {
            if (handler is IHttpHandlerProvider provider)
            {
                return provider.GetHandler<T>();
            }
            else if (handler is T tHander)
            {
                return tHander;
            }
            else
            {
                return default;
            }
        }
    }
}
