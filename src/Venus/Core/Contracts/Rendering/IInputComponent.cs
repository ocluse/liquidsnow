namespace Ocluse.LiquidSnow.Venus.Contracts.Rendering;

/// <summary>
/// Defines a component that collects user input.
/// </summary>
public interface IInputComponent : IElementComponent
{
    /// <summary>
    /// Gets or sets the class to apply when the input is disabled.
    /// </summary>
    string? DisabledClass { get; }

    /// <summary>
    /// Gets or sets the class to apply when the input is readonly.
    /// </summary>
    string? ReadOnlyClass { get; }

    /// <summary>
    /// Gets or sets the content to display before the input.
    /// </summary>
    RenderFragment? PrefixContent { get; set; }

    /// <summary>
    /// Gets or sets the content to display after the input.
    /// </summary>
    RenderFragment? SuffixContent { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the validation area of the input.
    /// </summary>
    RenderFragment<ValidationResult?>? ValidationContent { get; set; }

    /// <summary>
    /// Gets or sets the header of the input.
    /// </summary>
    string? Header { get; set; }

    ///// <summary>
    ///// Gets or sets the 'name' attribute of the input.
    ///// </summary>
    ///// <remarks>
    ///// If not provided the <see cref="Header"/> value will be used as the name, 
    ///// otherwise it falls back to a randomly generated string.
    ///// </remarks>
    //string? Name { get; set; }

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
