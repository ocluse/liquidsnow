using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;

public class FloatingActionButton : ClickableBase
{
    [Parameter]
    [EditorRequired]
    public string Icon { get; set; } = string.Empty;

    [Parameter]
    public string? Text { get; set; }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FeatherIcon>(1);
        {
            builder.AddAttribute(2, nameof(FeatherIcon.Icon), Icon);
        }
        builder.CloseComponent();

        if (!string.IsNullOrEmpty(Text))
        {
            builder.OpenElement(3, "span");
            {
                builder.AddContent(4, Text);
            }
            builder.CloseElement();
        }
    }

    protected override void BuildControlClass(ClassBuilder builder)
    {
        base.BuildControlClass(builder);
        builder.Add("floating-action-button");
    }
}