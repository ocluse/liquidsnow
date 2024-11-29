namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a component that collects user input.
/// </summary>
public interface IInputComponent : IElementComponent
{
    /// <summary>
    /// The class to apply when the input is disabled.
    /// </summary>
    string? DisabledClass { get; }

    /// <summary>
    /// The class to apply when the input is readonly.
    /// </summary>
    string? ReadOnlyClass { get; }

    /// <summary>
    /// The content to display before the input.
    /// </summary>
    RenderFragment? PrefixContent { get; }

    /// <summary>
    /// The content to display after the input.
    /// </summary>
    RenderFragment? SuffixContent { get; }

    /// <summary>
    /// The content to display in the validation area of the input.
    /// </summary>
    RenderFragment<ValidationResult?>? ValidationContent { get; }

    /// <summary>
    /// The header of the input.
    /// </summary>
    string? Header { get; }

    /// <summary>
    /// Gets the final name that is duly applied to the 'name' attribute of the component.
    /// </summary>
    string AppliedName { get; }

    /// <summary>
    /// Returns the validation result of the current data in the input.
    /// </summary>
    /// <returns></returns>
    ValidationResult? GetValidationResult();

    /// <summary>
    /// Returns the CSS class to apply to the validation label depending on the validation state.
    /// </summary>
    string? GetValidationClass();
}
