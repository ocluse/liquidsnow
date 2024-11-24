namespace Ocluse.LiquidSnow.Venus.Services;

internal class SnackbarService : ISnackbarService
{
    private ISnackbarHost? _host;

    private ISnackbarHost Host => _host ?? throw new InvalidOperationException("No Snackbar Host has been set");

    public void AddError(string message, SnackbarDuration duration = SnackbarDuration.Medium)
    {
        SnackbarMessage snackbarMessage = new(message, MessageStatus.Error, duration);

        Host.ShowMessage(snackbarMessage);
    }

    public void AddInformation(string message, SnackbarDuration duration = SnackbarDuration.Medium)
    {
        SnackbarMessage snackbarMessage = new(message, MessageStatus.Information, duration);

        Host.ShowMessage(snackbarMessage);
    }

    public void AddSuccess(string message, SnackbarDuration duration = SnackbarDuration.Medium)
    {
        SnackbarMessage snackbarMessage = new(message, MessageStatus.Success, duration);

        Host.ShowMessage(snackbarMessage);
    }

    public void AddWarning(string message, SnackbarDuration duration = SnackbarDuration.Medium)
    {
        SnackbarMessage snackbarMessage = new(message, MessageStatus.Warning, duration);

        Host.ShowMessage(snackbarMessage);
    }

    public void SetHost(ISnackbarHost host)
    {
        _host = host;
    }
}
