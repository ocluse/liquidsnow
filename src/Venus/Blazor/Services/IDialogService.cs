using Microsoft.AspNetCore.Components;
using Ocluse.LiquidSnow.Venus.Blazor.Contracts;

namespace Ocluse.LiquidSnow.Venus.Blazor.Services
{
    public interface IDialogService
    {
        void SetHost(IDialogHost host);
        Task<DialogResult> ShowDialog<T>(string? dialogHeader = null, bool allowDismiss = false, bool showClose = true, Dictionary<string, object>? parameters = null) where T : ComponentBase;
        Task<DialogResult> ShowDialog(Type dialogType, string? dialogHeader = null, bool allowDismiss = false, bool showClose = true, Dictionary<string, object>? parameters = null);
        void ShowLoading(string loadingMessage = "Loading...");
        void HideLoading();
    }
}
