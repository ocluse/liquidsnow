namespace Ocluse.LiquidSnow.Core.Tests.Events;

public class EventsApplication : SimpleApplication
{
    public override void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddEventBus();
        Ocluse.LiquidSnow.Core.Tests.Generated.Cqrs.ProjectEvents.AddListeners(builder);
        services.AddScoped<DisposableService>();
    }
}
