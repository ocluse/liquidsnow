using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Blazor.Contracts;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public abstract class ButtonBase : ControlBase, IInput, IDisposable
{
    private bool _disposedValue;

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

    [CascadingParameter]
    public IFormContainer? FormContainer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormContainer?.Register(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                FormContainer?.Unregister(this);
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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

        Dictionary<string, object> attributes = [];

        if (!Disabled)
        {
            attributes.Add("onclick", OnClick);
        }

        foreach (var attr in GetClassAndStyle())
        {
            attributes.Add(attr.Key, attr.Value);
        }

        if (Disabled)
        {
            attributes.Add("disabled", "disabled");
        }

        if (Href == null)
        {
            attributes.Add("role", "button");
        }
        else if (!Disabled)
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
