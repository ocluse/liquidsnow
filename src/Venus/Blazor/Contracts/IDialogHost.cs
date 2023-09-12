namespace Ocluse.LiquidSnow.Venus.Blazor.Contracts
{
    public interface IDialogHost
    {
        Task<DialogResult> ShowDialog(Type dialogType, string? dialogHeader, bool allowDismiss, bool showClose, Dictionary<string, object>? parameters);
        void CloseDialog(bool? isSuccess  = null, object? data = null);
        void ShowLoading(string loadingMessage);
        void HideLoading();
    }
}
