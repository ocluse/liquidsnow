
namespace Ocluse.LiquidSnow.Venus.Services;

internal class DialogService : IDialogService
{
    private IDialogHost? _host;

    private IDialogHost Host => _host ?? throw new InvalidOperationException("No Dialog Host has been set. " +
        "Try adding the <DialogHost> component to your Layout root." +
        " If you're using a custom DialogHost, ensure it is properly bound by calling BindHost on OnInitialized");

    public void BindHost(IDialogHost host)
    {
        _host = host;
    }

    public void UnbindHost(IDialogHost host)
    {
        if (_host == host)
        {
            _host = null;
        }
    }

    public async Task<DialogResult> ShowDialogAsync(DialogDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        return await Host.ShowDialogAsync(descriptor, cancellationToken);
    }
}
