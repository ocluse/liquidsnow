using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Internal;

internal class Dialog : VenusComponentBase, IDialog
{
    private ElementReference _dialogElement;
    private string? _customClasses;

    [Parameter]
    [EditorRequired]
    public required DialogDescriptor Descriptor { get; set; }

    [Parameter]
    [EditorRequired]
    public required IDialogHandle Handle { get; set; }

    [Inject]
    private IVenusJSInterop JSInterop { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        builder.OpenComponent<CascadingValue<IDialog>>(1);
        {
            builder.AddAttribute(2, nameof(CascadingValue<IDialog>.Value), this);
            builder.AddAttribute(3, nameof(CascadingValue<IDialog>.IsFixed), true);
            builder.AddAttribute(4, nameof(CascadingValue<IDialog>.ChildContent), (RenderFragment)BuildDialog);
        }
        builder.CloseComponent();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JSInterop.ShowDialogAsync(_dialogElement);
    }

    private void BuildDialog(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "dialog");
        {
            builder.AddAttribute(2, "class", ClassBuilder.Join(ClassNameProvider.Dialog, _customClasses));

            builder.AddElementReferenceCapture(3, (value) => _dialogElement = value);

            if (Descriptor.HeaderContentType is not null)
            {
                builder.OpenComponent(4, Descriptor.HeaderContentType);
                {
                    if (Descriptor.HeaderParameters is not null)
                    {
                        builder.AddMultipleAttributes(5, Descriptor.HeaderParameters!);
                    }
                }
                builder.CloseComponent();
            }

            builder.OpenComponent(6, Descriptor.ChildContentType);
            {
                if (Descriptor.ContentParameters is not null)
                {
                    builder.AddMultipleAttributes(7, Descriptor.ContentParameters!);
                }
            }
            builder.CloseComponent();

            if (Descriptor.FooterContentType is not null)
            {
                builder.OpenComponent(8, Descriptor.FooterContentType);
                {
                    if (Descriptor.FooterParameters is not null)
                    {
                        builder.AddMultipleAttributes(9, Descriptor.FooterParameters!);
                    }
                }
                builder.CloseComponent();
            }
        }
        builder.CloseElement();
    }

    public async Task UpdateDialogClassesAsync(string classes)
    {
        _customClasses = classes;
        await InvokeAsync(StateHasChanged);
    }

    public async Task UpdateDialogClassesAsync(ClassBuilder builder)
    {
        await UpdateDialogClassesAsync(builder.Build());
    }

    public async Task CloseAsync(bool? success, object? data)
    {
        await JSInterop.CloseDialogAsync(_dialogElement);
        await Handle.CloseAsync(success, data);
    }
}
