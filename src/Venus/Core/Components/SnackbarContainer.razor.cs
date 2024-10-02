namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays snackbar items.
/// </summary>
public partial class SnackbarContainer : ISnackbarHost
{
    private readonly List<SnackbarItemHandle> _items = [];

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        SnackbarService.SetHost(this);
    }

    ///<inheritdoc/>
    public ISnackbarItemHandle ShowMessage(SnackbarMessage message)
    {
        SnackbarItemHandle item = new()
        {
            Message = message.Content,
            Status = message.Status,
            Parent = this,
            Duration = message.Duration
        };

        AddItem(item);

        return item;
    }

    internal void RemoveItem(SnackbarItemHandle item)
    {
        _items.Remove(item);
        InvokeAsync(StateHasChanged);
    }

    private async void AddItem(SnackbarItemHandle item)
    {
        _items.Add(item);
        await InvokeAsync(StateHasChanged);
        int delay = Resolver.ResolveSnackbarDurationToMilliseconds(item.Duration);
        if (delay > 0)
        {
            await Task.Delay(delay);
            RemoveItem(item);
        }
    }
}