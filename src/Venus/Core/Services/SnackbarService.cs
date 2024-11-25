
namespace Ocluse.LiquidSnow.Venus.Services;

internal class SnackbarService : ISnackbarService
{
    private ISnackbarHost? _host;

    private ISnackbarHost Host => _host 
        ?? throw new InvalidOperationException("No Snackbar Host has been set." +
            "Ensure you include the <SnackbarHost> component in your root Layout" +
            "If you're using a custom SnackbarHost, ensure it is bound to the service.");

    public void BindHost(ISnackbarHost host)
    {
        _host = host;
    }

    public void UnbindHost(ISnackbarHost host)
    {
        if(_host == host)
        {
            _host = null;
        }
    }

    public async Task ShowSnackbarItemAsync(SnackbarItemDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        await Host.ShowSnackbarAsync(descriptor, cancellationToken);
    }

    
}
