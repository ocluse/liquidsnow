using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
