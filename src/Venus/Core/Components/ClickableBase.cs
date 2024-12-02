using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for clickable components.
/// </summary>
public abstract class ClickableBase : FormControlBase
{
    /// <summary>
    /// Gets or sets the callback that is invoked when the component is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// Gets or sets the a link to navigate to when the component is clicked.
    /// </summary>
    /// <remarks>
    /// When this property is null, the control is rendered as html button.
    /// </remarks>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets where to open the linked document when the component is clicked.
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Gets or sets a class to apply when the component is disabled.
    /// </summary>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets a value that determines whether click events should not be propagated.
    /// </summary>
    [Parameter]
    public bool StopPropagation { get; set; }

    /// <summary>
    /// Adds CSS classes that will be applied to the component to the supplied <see cref="ClassBuilder"/>.
    /// </summary>
    protected virtual void BuildControlClass(ClassBuilder builder) { }

    ///<inheritdoc/>
    protected override sealed void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);

        BuildControlClass(builder);

        if (Disabled)
        {
            builder.Add(DisabledClass ?? ClassNameProvider.ComponentDisabled);
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

        if (Disabled)
        {
            attributes.Add("disabled", "disabled");
        }

        if (Href == null)
        {
            attributes.Add("role", "button");
        }
        else if (!Disabled && Href.IsNotEmpty())
        {
            attributes.Add("href", Href);

            if (Target.IsNotEmpty())
            {
                attributes.Add("target", Target);
            }
        }
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        string elementName = Href == null ? "button" : "a";

        builder.OpenElement(1, elementName);
        {
            builder.AddMultipleAttributes(2, GetAttributes());
            builder.AddEventStopPropagationAttribute(3, "onclick", StopPropagation);
            builder.OpenRegion(4);
            {
                BuildContent(builder);
            }
            builder.CloseRegion();
        }
        builder.CloseElement();
    }

    /// <summary>
    /// Renders the inner content to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    protected abstract void BuildContent(RenderTreeBuilder builder);
}
