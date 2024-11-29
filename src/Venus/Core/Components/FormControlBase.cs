namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for interactable components that can be hosted in forms.
/// </summary>
public abstract class FormControlBase : ControlBase, IFormControl, IDisposable
{
    private bool _disposedValue;

    [CascadingParameter]
    private IForm? ParentForm { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public bool Disabled { get; set; }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ParentForm?.Register(this);
    }

    ///<inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ParentForm?.Unregister(this);
            }

            _disposedValue = true;
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
