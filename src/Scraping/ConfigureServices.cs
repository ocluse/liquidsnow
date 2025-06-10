using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocluse.LiquidSnow.Scraping.Internal;

namespace Ocluse.LiquidSnow.Scraping;

/// <summary>
/// Extensions for adding the scarping utilities to the DI container.
/// </summary>
public static class ConfigureServices
{
    ///<inheritdoc cref="AddScrapingBrowser(IServiceCollection, Action{BrowserOptions})"/>
    public static IHttpClientBuilder AddScrapingBrowser(this IServiceCollection services)
    {
        return AddScrapingBrowser(services, BrowserHeaders.DefaultUserAgent);
    }

    ///<inheritdoc cref="AddScrapingBrowser(IServiceCollection, Action{BrowserOptions})"/>
    public static IHttpClientBuilder AddScrapingBrowser(this IServiceCollection services, string userAgent)
    {
        return AddScrapingBrowser(services, options => options.UserAgent = userAgent);
    }

    /// <summary>
    /// Adds the scraping browser, that emulates real browsers, to the container.
    /// </summary>
    public static IHttpClientBuilder AddScrapingBrowser(this IServiceCollection services, Action<BrowserOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddTransient(sp=>
        {
            var factory = sp.GetRequiredService<IBrowserFactory>();
            var options = sp.GetRequiredService<IOptions<BrowserOptions>>().Value;
            return factory.CreateBrowser(options);
        });

        services.AddSingleton<IBrowserFactory, BrowserFactory>();

        return services.AddHttpClient(Browser.HTTPCLIET_NAME)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                UseCookies = false
            });
    }
}
