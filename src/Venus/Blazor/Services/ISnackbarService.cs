using Ocluse.LiquidSnow.Venus.Blazor.Contracts;

namespace Ocluse.LiquidSnow.Venus.Blazor.Services
{
    public interface ISnackbarService
    {
        void SetHost(ISnackbarHost host);

        void AddError(string message, SnackbarDuration duration = SnackbarDuration.Medium);
        
        void AddSuccess(string message, SnackbarDuration duration = SnackbarDuration.Medium);
        
        void AddInformation(string message, SnackbarDuration duration = SnackbarDuration.Medium);
        
        void AddWarning(string message, SnackbarDuration duration = SnackbarDuration.Medium);
    }
}
