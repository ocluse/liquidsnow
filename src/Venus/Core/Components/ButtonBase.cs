using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for a button component.
/// </summary>
public abstract class ButtonBase : ControlBase, IInput, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Callback that is invoked when the button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// The URL to navigate to when the button is clicked.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// The target of the URL when the button is clicked.
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets whether the button is disabled and cannot be interacted with.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// The class to apply when the button is disabled.
    /// </summary>
    /// <remarks>
    /// If not provided, a default 'disabled' class will be added instead
    /// </remarks>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// The parent form, if it exists.
    /// </summary>
    [CascadingParameter]
    public IForm? FormContainer { get; private set; }

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormContainer?.Register(this);
    }

    ///<inheritdoc cref="Dispose()"/>
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

    ///<inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Called to add additional classes to the button by inheriting classes.
    /// </summary>
    protected virtual void BuildButtonClass(ClassBuilder classBuilder) { }

    ///<inheritdoc/>
    protected override sealed void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);

        BuildButtonClass(classBuilder);

        if (Disabled)
        {
            classBuilder.Add(DisabledClass ?? "disabled");
        }
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);

        if (!Disabled)
        {
            attributes.Add("onclick", OnClick);
        }

        foreach (var attr in GetAttributes())
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
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "a");

        builder.AddMultipleAttributes(1, GetAttributes());

        BuildContent(builder);

        builder.CloseElement();
    }

    /// <summary>
    /// Called to build the inner content of the button.
    /// </summary>
    protected abstract void BuildContent(RenderTreeBuilder builder);


}
