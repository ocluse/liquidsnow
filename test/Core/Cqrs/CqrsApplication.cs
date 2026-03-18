namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public class CqrsApplication : SimpleApplication
{
    public override void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddCqrs();
        Ocluse.LiquidSnow.Core.Tests.Generated.Cqrs.ProjectCqrs.AddHandlers(builder);
    }
}
