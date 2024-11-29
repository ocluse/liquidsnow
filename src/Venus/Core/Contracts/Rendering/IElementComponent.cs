namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a component that renders HTML.
/// </summary>
public interface IElementComponent : IVenusComponent
{
    /// <summary>
    /// The padding applied to the element.
    /// </summary>
    string? Padding { get; }

    /// <summary>
    /// The unit for the spacing values.
    /// </summary>
    CssUnit? SpacingUnit { get; }

    /// <summary>
    /// The margin applied to the element.
    /// </summary>
    string? Margin { get; }

    /// <summary>
    /// The CSS class applied to the element.
    /// </summary>
    string? Class { get; }

    /// <summary>
    /// The inline style applied to the element.
    /// </summary>
    string? Style { get; }

    /// <summary>
    /// The title applied to the element.
    /// </summary>
    string? Title { get; }

    /// <summary>  
    /// Additional attributes to apply to the element.
    /// </summary>  
    IReadOnlyDictionary<string, object>? AdditionalAttributes { get; }

    /// <summary>
    /// Returns the attributes to be applied to the component, including the style, class and any other specified by an inheriting component.
    /// </summary>
    Dictionary<string, object> GetAttributes();
}
