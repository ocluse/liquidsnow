namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The default host component for displaying dialogs.
/// </summary>
public partial class DialogHost : IDialogHost
{
    private bool _showDialog;
    private bool _showLoading;
    private bool _showClose;
    private string? _loadingMessage;
    private Type? _dialogType;
    private bool _allowDismiss;
    private string? _dialogHeader;

    private Dictionary<string, object?>? _dialogParameters;

    private TaskCompletionSource<DialogResult>? _tcsDialog;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        DialogService.BindHost(this);
    }

    ///<inheritdoc/>
    public async Task<DialogResult> ShowDialog(Type dialogType, string? dialogHeader, bool allowDismiss, bool showClose, Dictionary<string, object?>? parameters)
    {
        _showClose = showClose;
        _allowDismiss = allowDismiss;
        _dialogHeader = dialogHeader;
        _tcsDialog = new();
        _dialogType = dialogType;
        _dialogParameters = parameters;
        _showDialog = true;
        await InvokeAsync(StateHasChanged);

        DialogResult obj = await _tcsDialog.Task;

        _showDialog = false;
        _dialogType = null;
        _dialogParameters = null;
        await InvokeAsync(StateHasChanged);
        return obj;
    }

    private void OnDismiss()
    {
        if (_allowDismiss)
        {
            CloseDialog();
        }
    }

    ///<inheritdoc/>
    public void CloseDialog(bool? isSuccess = null, object? data = null)
    {
        DialogResult result = new()
        {
            Data = data,
            Success = isSuccess
        };

        _tcsDialog?.SetResult(result);
    }

    ///<inheritdoc/>
    public void ShowLoading(string? loadingMessage)
    {
        _loadingMessage = loadingMessage;
        _showLoading = true;
        InvokeAsync(StateHasChanged);
    }

    ///<inheritdoc/>
    public void HideLoading()
    {
        _showLoading = false;
        InvokeAsync(StateHasChanged);
    }
}