using Ocluse.LiquidSnow.Venus.Blazor.Components;
using Ocluse.LiquidSnow.Venus.Blazor.Contracts;

namespace Ocluse.LiquidSnow.Venus.Blazor.Models
{
    internal class SnackbarItemHandle : ISnackbarItemHandle
    {
        public required string Message { get; init; }
        public required int Status { get; init; }
        public required SnackbarDuration Duration { get; init; }
        public required SnackbarContainer Parent { get; init; }

        public void Close()
        {
            Parent.RemoveItem(this);
        }
    }
}
