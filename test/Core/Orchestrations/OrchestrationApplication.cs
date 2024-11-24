namespace Ocluse.LiquidSnow.Core.Tests.Orchestrations;

public class OrchestrationApplication : SimpleApplication
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddOrchestrator();
    }
}
