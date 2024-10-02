using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Models;

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
