using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a <see cref="IFieldComponent"/> that renders separate auxiliary content.
/// </summary>
public interface IAuxiliaryContentFieldComponent : IFieldComponent
{
    /// <summary>
    /// Renders the auxiliary content to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    void BuildAuxiliaryContent(RenderTreeBuilder builder);
}
