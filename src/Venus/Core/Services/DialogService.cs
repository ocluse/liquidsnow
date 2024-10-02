namespace Ocluse.LiquidSnow.Venus.Services;

internal class DialogService : IDialogService
{
    private IDialogHost? _host;

    private IDialogHost Host => _host ?? throw new InvalidOperationException("No Dialog Host has been set");

    public void BindHost(IDialogHost host)
    {
        _host = host;
    }

    public Task<DialogResult> ShowDialogAsync(Type dialogType, string? dialogHeader = null, bool allowDismiss = false, bool showClose = true, Dictionary<string, object?>? parameters = null)
    {
        return Host.ShowDialog(dialogType, dialogHeader, allowDismiss, showClose, parameters);
    }

    public void ShowLoading(string? loadingMessage)
    {
        Host.ShowLoading(loadingMessage);
    }

    public void HideLoading()
    {
        Host.HideLoading();
    }
}
