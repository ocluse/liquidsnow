namespace Ocluse.LiquidSnow.Core.Tests.Events;

public class EventsApplication : SimpleApplication
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddEventBus();
        services.AddScoped<DisposableService>();
    }
}
