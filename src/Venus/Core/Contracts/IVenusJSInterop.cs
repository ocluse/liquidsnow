using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Provides methods for interacting with the Venus JavaScript module.
/// </summary>
public interface IVenusJSInterop
{
    /// <summary>
    /// Closes the supplied dialog.
    /// </summary>
    ValueTask CloseDialogAsync(ElementReference dialog);

    /// <summary>
    /// Shows the supplied dialog.
    /// </summary>
    ValueTask ShowDialogAsync(ElementReference dialog);

    /// <summary>
    /// Shows the supplied popover.
    /// </summary>
    ValueTask ShowPopoverAsync(ElementReference element);

    /// <summary>
    /// Hides the supplied popover.
    /// </summary>
    ValueTask HidePopoverAsync(ElementReference element);
}