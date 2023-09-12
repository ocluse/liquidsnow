using Ocluse.LiquidSnow.Venus.Blazor.Models;

namespace Ocluse.LiquidSnow.Venus.Blazor.Contracts
{
    public interface ISnackbarHost
    {
        ISnackbarItemHandle ShowMessage(SnackbarMessage message);
    }
}
