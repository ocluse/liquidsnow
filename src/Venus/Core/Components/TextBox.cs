using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that collects a <see cref="string"/> value.
/// </summary>
public class TextBox : TextBoxBase<string>
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
