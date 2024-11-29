using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Internal;

internal class SnackbarItem : ComponentBase, ISnackbarItem
{
    [Parameter]
    [EditorRequired]
    public required ISnackbarItemHandle Handle { get; set; }

    [Parameter]
    public required SnackbarItemDescriptor Descriptor { get; set; }

    public Task CloseAsync()
    {
        throw new NotImplementedException();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        builder.OpenComponent<CascadingValue<ISnackbarItem>>(1);
        {
            builder.AddAttribute(2, nameof(CascadingValue<ISnackbarItem>.Value), this);
            builder.AddAttribute(3, nameof(CascadingValue<ISnackbarItem>.IsFixed), true);
            builder.AddAttribute(4, nameof(CascadingValue<ISnackbarItem>.ChildContent), (RenderFragment)BuildSnackbarItem);
        }
        builder.CloseComponent();
    }

    private void BuildSnackbarItem(RenderTreeBuilder builder)
    {
        builder.OpenComponent(1, Descriptor.ContentType);
        {
            if (Descriptor.Parameters is not null)
            {
                builder.AddMultipleAttributes(2, Descriptor.Parameters!);
            }
        }
        builder.CloseComponent();
    }
}