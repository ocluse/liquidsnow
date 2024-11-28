using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Internal;

internal class SnackbarMessageView : SnackbarItemComponentBase
{
    [Parameter]
    [EditorRequired]
    public required string Content { get; set; }

    [Parameter]
    public int Status { get; set; }

    [Parameter]
    public bool ShowClose { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        string? css = ClassBuilder.Join(
            ClassNameProvider.SnackbarMessage,
            Resolver.ResolveSnackbarStatusToClass(Status)
            );

        int color = Resolver.ResolveSnackbarStatusToColor(Status);

        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", css);

            Type iconComponentType = Resolver.GetIconComponentType();

            string icon = Resolver.ResolveSnackbarStatusToIcon(Status);

            builder.OpenComponent(3, iconComponentType);
            {
                builder.AddAttribute(4, nameof(ISvgIcon.Icon), icon);
                builder.AddAttribute(5, nameof(ISvgIcon.Size), Resolver.SnackbarIconSize);
                builder.AddAttribute(6, nameof(ControlBase.Color), color);
            }
            builder.CloseComponent();

            builder.OpenElement(7, "div");
            {
                builder.AddAttribute(8, "class", ClassNameProvider.SnackbarMessage_Content);
                builder.OpenElement(9, "span");
                {
                    builder.AddContent(10, Content);
                }
                builder.CloseElement();
            }
            builder.CloseElement();

            if (ShowClose)
            {
                Type buttonComponentType = Resolver.GetIconButtonComponentType();
                icon = ComponentIcons.Get(Resolver.IconStyle, ComponentIcon.Close);

                builder.OpenComponent(4, buttonComponentType);
                {
                    builder.AddAttribute(5, nameof(ISvgIcon.Icon), icon);
                    builder.AddAttribute(6, "class", ClassNameProvider.SnackbarMessage_CloseButton);
                    builder.AddAttribute(7, nameof(ClickableBase.OnClick), EventCallback.Factory.Create(this, HandleCloseAsync));
                }
                builder.CloseComponent();
            }
        }
        builder.CloseElement();
    }

    public async Task HandleCloseAsync()
    {
        await Item.CloseAsync();
    }
}
