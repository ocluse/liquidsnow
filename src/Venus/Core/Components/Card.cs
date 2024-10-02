using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a card that can contain other components.
/// </summary>
public class Card : ControlBase
{
    /// <summary>
    /// Gets or sets the content of the card.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content of the card header.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets the content of the card footer.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card content.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card footer.
    /// </summary>
    [Parameter]
    public string? FooterClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("card");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, GetAttributes());

        if (HeaderContent != null)
        {
            string headerClass = HeaderClass ?? "card_header";

            builder.OpenElement(3, "div");
            builder.AddAttribute(4, "class", headerClass);
            builder.AddContent(5, HeaderContent);
            builder.CloseElement();
        }

        string contentClass = ContentClass ?? "card_body";
        builder.OpenElement(6, "div");
        builder.AddAttribute(7, "class", contentClass);
        builder.AddContent(8, ChildContent);
        builder.CloseElement();

        if (FooterContent != null)
        {
            string footerClass = FooterClass ?? "card_footer";

            builder.OpenElement(9, "div");
            builder.AddAttribute(10, "class", footerClass);
            builder.AddContent(11, FooterContent);
            builder.CloseElement();
        }
        builder.CloseElement();
    }
}
