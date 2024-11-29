using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// An input component that collects text input.
/// </summary>
public class StaticTextBox : StaticTextBoxBase<string>
{
    /// <summary>
    /// Gets or sets the keyboard that determines the collectable value type.
    /// </summary>
    [Parameter]
    public Keyboard Keyboard { get; set; }

    ///<inheritdoc/>
    protected override string InputType => Keyboard.ToString().PascalToKebabCase();
    
    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;

        result ??= string.Empty;

        return true;
    }
}
