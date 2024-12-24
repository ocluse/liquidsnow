using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Components.Internal;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A snackbar host that renders multiple snackbar items.
/// </summary>
public sealed class SnackbarHost : ControlBase, ISnackbarHost, IAsyncDisposable
{
    private readonly List<SnackbarItemHandle> _handles = [];

    [Inject]
    private ISnackbarService SnackbarService { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        SnackbarService.BindHost(this);
    }

    /// <inheritdoc />
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.SnackbarHost);
    }

    /// <inheritdoc />
    public async Task ShowSnackbarAsync(SnackbarItemDescriptor descriptor, CancellationToken cancellationToken)
    {
        while(_handles.Count > Resolver.MaxSnackbarItems && Resolver.MaxSnackbarItems > 0)
        {
            await Task.Delay(100, cancellationToken);
        }

        SnackbarItemHandle handle = new(descriptor, cancellationToken);

        _handles.Add(handle);

        await InvokeAsync(StateHasChanged);

        var durationMillis = Resolver.ResolveSnackbarDurationToMilliseconds(descriptor.Duration);

        handle.StartCountdown(durationMillis);

        await handle.CompletionSource.Task;

        _handles.Remove(handle);

        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            foreach (SnackbarItemHandle handle in _handles)
            {
                builder.OpenComponent<SnackbarItem>(3);
                {
                    builder.SetKey(handle);
                    builder.AddAttribute(4, nameof(SnackbarItem.Descriptor), handle.Descriptor);
                    builder.AddAttribute(5, nameof(SnackbarItem.Handle), handle);
                }
                builder.CloseComponent();
            }
        }
        builder.CloseElement();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_handles.Count > 0)
        {
            List<SnackbarItemHandle> handles = [.. _handles];
            foreach (var item in handles)
            {
                await item.CloseAsync();
            }
        }

        SnackbarService.UnbindHost(this);
    }

    private class SnackbarItemHandle(SnackbarItemDescriptor descriptor, CancellationToken cancellationToken) : ISnackbarItemHandle
    {
        private bool _isCounting;
        public SnackbarItemDescriptor Descriptor { get; } = descriptor;

        public TaskCompletionSource CompletionSource { get; } = new();

        public async void StartCountdown(int durationMillis)
        {
            if (_isCounting)
            {
                throw new InvalidOperationException("The snackbar item is already counting down");
            }

            _isCounting = true;

            if (durationMillis > 0)
            {
                await Task.Delay(durationMillis, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    CompletionSource.TrySetResult();
                }
            }
        }

        public Task CloseAsync()
        {
            CompletionSource.TrySetResult();
            return Task.CompletedTask;
        }
    }
}
