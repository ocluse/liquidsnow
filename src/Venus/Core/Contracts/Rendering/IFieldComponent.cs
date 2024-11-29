using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a component rendered as a field.
/// </summary>
public interface IFieldComponent : IInputComponent
{
    /// <summary>
    /// Gets or sets the style of the header.
    /// </summary>
    FieldHeaderStyle? HeaderStyle { get; set; }

    /// <summary>
    /// Gets or sets the class applied to the header of the input.
    /// </summary>
    string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the header of the input.
    /// </summary>
    RenderFragment<string>? HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the input's content area.
    /// </summary>
    string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the prefix content containing div.
    /// </summary>
    string? PrefixClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the suffix content containing div.
    /// </summary>
    string? SuffixClass { get; set; }

    /// <summary>
    /// Renders the actual input element to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    void BuildInput(RenderTreeBuilder builder);
}
