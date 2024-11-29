using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// An input component that collects text input.
/// </summary>
public class StaticTextBox : StaticInputBase<string>
{
    /// <summary>
    /// Gets or sets a placeholder for the component
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotImplementedException();
    }
}
