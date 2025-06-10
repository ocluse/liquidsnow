namespace Ocluse.LiquidSnow.Scraping.Internal;

internal class BrowserFactory(IHttpClientFactory httpClientFactory) : IBrowserFactory
{
    public IBrowser CreateBrowser(BrowserOptions options)
    {
        return new Browser(httpClientFactory, options);
    }
}
