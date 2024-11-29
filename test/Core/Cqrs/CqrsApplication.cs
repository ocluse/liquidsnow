namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public class CqrsApplication : SimpleApplication
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddCqrs();
    }
}
