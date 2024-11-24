using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Services;

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
    /// Initializes the dropdown watcher that notifies dropdowns when they should be closed.
    /// </summary>
    /// <remarks>
    /// This method must be called at least once for dropdowns to work correctly.
    /// A standard <see cref="Dropdown{T}"/> already calls the method.
    /// </remarks>
    ValueTask InitializeDropdownWatcher();

    /// <summary>
    /// Stops watching the supplied dialog for close events.
    /// </summary>
    ValueTask UnwatchDropdownAsync(DotNetObjectReference<IDropdown> dropdown);

    /// <summary>
    /// Starts watching the supplied dialog for close events.
    /// </summary>
    ValueTask WatchDropdownAsync(DotNetObjectReference<IDropdown> dropdown);
}