namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a base class for Venus components.
/// </summary>
public interface IVenusComponent : IComponent
{
    /// <summary>
    /// Gets or sets the component's Resolver.
    /// </summary>
    IVenusResolver Resolver { get; }

    /// <summary>
    /// Gets or sets the component's CSS class name provider.
    /// </summary>
    IClassNameProvider ClassNameProvider { get; }
}
