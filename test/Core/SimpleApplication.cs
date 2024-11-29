using Microsoft.Extensions.Hosting;

namespace Ocluse.LiquidSnow.Core.Tests;

public abstract class SimpleApplication : IAsyncLifetime
{
    public IHost Host { get; private set; } = default!;

    public async Task DisposeAsync()
    {
        await Host.StopAsync();
    }

    public abstract void ConfigureServices(IServiceCollection services);

    public async Task InitializeAsync()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateOnBuild = true;
                options.ValidateScopes = true;
            });

        builder.ConfigureServices(ConfigureServices);

        Host = builder.Build();

        await Host.StartAsync();
    }
}
