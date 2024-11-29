using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Components.Internal;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A dialog host that is able to show multiple dialogs.
/// </summary>
public sealed class DialogHost : ControlBase, IDialogHost, IAsyncDisposable
{
    private readonly List<DialogHandle> _handles = [];

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DialogService.BindHost(this);
    }

    /// <inheritdoc />
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.DialogHost);
    }

    /// <inheritdoc />
    public async Task<DialogResult> ShowDialogAsync(DialogDescriptor descriptor, CancellationToken cancellationToken)
    {
        DialogHandle handle = new(descriptor);

        _handles.Add(handle);

        await InvokeAsync(StateHasChanged);

        cancellationToken.Register(handle.CompletionSource.SetCanceled);

        var result = await handle.CompletionSource.Task;

        _handles.Remove(handle);

        await InvokeAsync(StateHasChanged);

        return result;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            foreach(DialogHandle handle in _handles)
            {
                builder.OpenComponent<Dialog>(3);
                {
                    builder.SetKey(handle);
                    builder.AddAttribute(4, nameof(Dialog.Handle), handle);
                    builder.AddAttribute(5, nameof(Dialog.Descriptor), handle.Descriptor);
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
            List<DialogHandle> handles = [.. _handles];
            foreach (var item in handles)
            {
                await item.CloseAsync(null, null);
            }
        }

        DialogService.UnbindHost(this);
    }

    private class DialogHandle(DialogDescriptor descriptor) : IDialogHandle
    {
        public DialogDescriptor Descriptor { get; } = descriptor;

        public TaskCompletionSource<DialogResult> CompletionSource { get; } = new();

        public Task CloseAsync(bool? success, object? data)
        {
            DialogResult result = new(success, data);
            CompletionSource.SetResult(result);
            return Task.CompletedTask;
        }
    }
}
