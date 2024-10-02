using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that collects user input as text.
/// </summary>
public class TextBox : InputControlBase<string>
{
    /// <summary>
    /// Gets or sets the keyboard that determines the collectable value type.
    /// </summary>
    [Parameter]
    public Keyboard Keyboard { get; set; }

    ///<inheritdoc/>
    protected override string InputType => Keyboard.ToString().PascalToKebabCase();

    ///<inheritdoc/>
    protected override string? GetValue(object? value)
    {
        return value?.ToString() ?? string.Empty;
    }
}
