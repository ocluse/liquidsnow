using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// CSS classes that are applied to child components of a <see cref="InputControlBase{T}"/>
/// </summary>
public record InputClasses
{
    /// <summary>
    /// The CSS style applied to the main content area, typically the bordered area around the input.
    /// </summary>
    public string? ContentArea { get; init; }

    /// <summary>
    /// The CSS style applied to the prefix content, typically the content that appears before the input.
    /// </summary>
    public string? PrefixContent { get; init; }

    /// <summary>
    /// The CSS style applied to the suffix content, typically the content that appears after the input.
    /// </summary>
    public string? SuffixContent { get; init; }

    /// <summary>
    /// The CSS style applied to the header label of the input.
    /// </summary>
    public string? HeaderLabel { get; init; }

    /// <summary>
    /// The CSS style applied to the actual input element.
    /// </summary>
    public string? Input { get; init; }

    /// <summary>
    /// The CSS style applied to the default validation label.
    /// </summary>
    public string? ValidationLabel { get; init; }
}
