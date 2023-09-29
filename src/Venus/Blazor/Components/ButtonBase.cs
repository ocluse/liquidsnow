using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public abstract class ButtonBase : ControlBase
{
    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter]
    public string? Href { get; set; }

    [Parameter]
    public string? Target { get; set; }

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public string? DisabledClass { get; set; }

    protected virtual void BuildButtonClass(ClassBuilder classBuilder)
    {

    }

    protected override sealed void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);

        BuildButtonClass(classBuilder);

        if (Disabled)
        {
            classBuilder.Add(DisabledClass ?? "disabled");
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "a");
        Dictionary<string, object> attributes = new()
        {
            {"onclick", OnClick },
        };

        foreach(var attr in GetClassAndStyle())
        {
            attributes.Add(attr.Key, attr.Value);
        }

        if(Disabled)
        {
            attributes.Add("disabled", "disabled");
        }

        if (Href == null)
        {
            attributes.Add("role", "button");
        }
        else
        {
            attributes.Add("href", Href);

            if (!string.IsNullOrEmpty(Target))
            {
                attributes.Add("target", Target);
            }
        }

        builder.AddMultipleAttributes(1, attributes);

        BuildContent(builder);

        builder.CloseElement();
    }

    protected abstract void BuildContent(RenderTreeBuilder builder);
}
