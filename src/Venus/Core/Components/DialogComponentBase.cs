using Ocluse.LiquidSnow.Venus.Components.Internal;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a dialog.
/// </summary>
public class DialogComponentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the component that is hosting the dialog.
    /// </summary>
    [CascadingParameter]
    protected IDialogHost DialogHost { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the dialog component that is being hosted and contains this component.
    /// </summary>
    [CascadingParameter]
    protected Dialog Dialog { get; private set; } = null!;

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    protected void Close(bool success, object? data = null)
    {
        DialogHost.CloseDialog(success, data);
    }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateClass();
    }

    /// <summary>
    /// Allows derived components to specify CSS styles to be added to the dialog.
    /// </summary>
    protected virtual void BuildClass(ClassBuilder builder) { }

    /// <summary>
    /// Called by derived classes to rebuild the dialog CSS classes.
    /// </summary>
    protected void UpdateClass()
    {
        ClassBuilder builder = new();
        BuildClass(builder);
    }
}
