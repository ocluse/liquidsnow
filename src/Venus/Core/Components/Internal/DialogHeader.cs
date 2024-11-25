using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components.Internal;

internal class DialogHeader : DialogComponentBase
{
    [Parameter]
    [EditorRequired]
    public required DialogHeaderOptions Options { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        
        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", ClassNameProvider.Dialog_Header);

            if (Options.Title.IsNotEmpty())
            {
                string elementName = Resolver.ResolveTextHierarchy(Options.HeaderHierarchy);

                builder.OpenElement(3, elementName);
                {
                    builder.AddAttribute(4, "class", ClassNameProvider.Dialog_Header_Title);
                    builder.AddContent(5, Options.Title);
                }
                builder.CloseElement();
            }

            if (Options.ShowClose)
            {
                Type componentType = Resolver.IconStyle == IconStyle.Feather 
                    ? typeof(FeatherIconButton) 
                    : typeof(FluentIconButton);

                string icon = Resolver.IconStyle == IconStyle.Feather
                    ? Resolver.ResolveSnackbarStatusToIcon(MessageStatus.Error)
                    : Resolver.ResolveSnackbarStatusToIcon(MessageStatus.Error);

                builder.OpenComponent(6, componentType);
                {
                    builder.AddAttribute(7, nameof(ISvgIcon.Icon), icon);
                    builder.AddAttribute(8, "class", ClassNameProvider.Dialog_Header_CloseButton);
                    builder.AddAttribute(9, nameof(ClickableBase.OnClick), EventCallback.Factory.Create(this, HandleClickClose));
                }
            }
        }
        builder.CloseElement();
    }

    protected async Task HandleClickClose()
    {
        await Dialog.CloseAsync(null, false);
    }
}
