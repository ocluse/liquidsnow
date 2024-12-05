using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a component rendered as a field.
/// </summary>
public interface IFieldComponent : IInputComponent
{
    /// <summary>
    /// The ID of the input.
    /// </summary>
    string? Id { get; }
    /// <summary>
    /// The style of the header.
    /// </summary>
    FieldHeaderStyle? HeaderStyle { get; }

    /// <summary>
    /// The class applied to the header of the input.
    /// </summary>
    string? HeaderClass { get; }

    /// <summary>
    /// The content to display in the header of the input.
    /// </summary>
    RenderFragment<string>? HeaderContent { get; }

    /// <summary>
    /// The CSS class applied to the input's content area.
    /// </summary>
    string? ContentClass { get; }

    /// <summary>
    /// The CSS class applied to the prefix content containing div.
    /// </summary>
    string? PrefixClass { get; }

    /// <summary>
    /// The CSS class applied to the suffix content containing div.
    /// </summary>
    string? SuffixClass { get; }

    /// <summary>
    /// Specifies whether the validation label should always be rendered even without a validation message.
    /// </summary>
    bool? AlwaysRenderValidationLabel { get; }

    /// <summary>
    /// Renders the actual input element to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    void BuildInput(RenderTreeBuilder builder);
}
