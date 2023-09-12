using Microsoft.AspNetCore.Components;
using Ocluse.LiquidSnow.Venus.Blazor.Contracts;

namespace Ocluse.LiquidSnow.Venus.Blazor.Services.Implementations
{
    internal class DialogService : IDialogService
    {
        private IDialogHost? _host;

        private IDialogHost GetHost()
        {
            if (_host == null)
            {
                throw new InvalidOperationException("No Dialog Host has been set");
            }
            else
            {
                return _host;
            }
        }
        
        public void SetHost(IDialogHost host)
        {
            _host = host;
        }

        public Task<DialogResult> ShowDialog<T>(string? dialogHeader = null, bool allowDismiss = false, bool showClose = true, Dictionary<string, object>? parameters = null) where T : ComponentBase
        {
            Type type = typeof(T);
            return ShowDialog(type, dialogHeader, allowDismiss, showClose, parameters);
        }

        public Task<DialogResult> ShowDialog(Type dialogType, string? dialogHeader = null, bool allowDismiss = false, bool showClose = true, Dictionary<string, object>? parameters = null)
        {
            return GetHost().ShowDialog(dialogType, dialogHeader, allowDismiss, showClose, parameters);
        }

        public void ShowLoading(string loadingMessage = "Loading...")
        {
            GetHost().ShowLoading(loadingMessage);
        }

        public void HideLoading()
        {
            GetHost().HideLoading();
        }
    }
}
