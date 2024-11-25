namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for Venus based components, provides quick access to common services.
/// </summary>
public class VenusComponentBase : ComponentBase
{
    /// <summary>
    /// [Injected] The famed and fabulous Venus resolver.
    /// </summary>
    [Inject]
    public IVenusResolver Resolver { get; private set; } = null!;

    /// <summary>
    /// [Injected] Gets or sets the CSS class name provider.
    /// </summary>
    [Inject]
    public ClassNameProvider ClassNameProvider { get; private set; } = null!;
}
