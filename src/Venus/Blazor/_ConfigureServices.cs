using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Venus.Blazor.Services;
using Ocluse.LiquidSnow.Venus.Blazor.Services.Implementations;

namespace Ocluse.LiquidSnow.Venus.Blazor
{
    public static class ConfigureServices
    {
        public static VenusServiceBuilder AddComponents(this VenusServiceBuilder builder)
        {
            return builder.AddComponents<BlazorResolver>();
        }

        public static VenusServiceBuilder AddComponents<T>(this VenusServiceBuilder builder)
            where T: class, IBlazorResolver
        {
            builder.Services.RemoveAll(typeof(IDialogService));
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<ISnackbarService, SnackbarService>();
            builder.Services.RemoveAll(typeof(IBlazorResolver));
            builder.Services.AddSingleton<IBlazorResolver, T>();

            return builder;
        }

        public static VenusServiceBuilder AddVenusComponents(this IServiceCollection services)
        {
            return services.AddVenus()
                .AddComponents();
        }
    }
}
