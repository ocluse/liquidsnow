namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base component for dialog components.
/// </summary>
public abstract class DialogComponentBase : VenusComponentBase
{
    /// <summary>
    /// Gets or sets the dialog component that is being hosted and contains this component.
    /// </summary>
    [CascadingParameter]
    public IDialog Dialog { get; private set; } = null!;
}

/// <summary>
/// The base component for snackbar components.
/// </summary>
public abstract class SnackbarItemComponentBase : VenusComponentBase
{
    /// <summary>
    /// Gets or sets the snackbar component that is containing this component.
    /// </summary>
    [CascadingParameter]
    public ISnackbarItem Item { get; private set; } = null!;
}