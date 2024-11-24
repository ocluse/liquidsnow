using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A scaffolding for card controls.
/// </summary>
public class Card : ControlBase
{
    /// <summary>
    /// Gets or sets the content of the card body.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card content.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets the content of the card header.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card header.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the content of the card footer.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the div containing the card footer.
    /// </summary>
    [Parameter]
    public string? FooterClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.Card);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            if (HeaderContent != null)
            {
                builder.OpenElement(3, "div");
                {
                    builder.AddAttribute(4, "class", ClassBuilder.Join(ClassNameProvider.Card_Header, HeaderClass));
                    builder.AddContent(5, HeaderContent);
                }
                builder.CloseElement();
            }

            builder.OpenElement(6, "div");
            {
                builder.AddAttribute(7, "class", ClassBuilder.Join(ClassNameProvider.Card_Content, ContentClass));
                builder.AddContent(8, ChildContent);
            }
            builder.CloseElement();

            if (FooterContent != null)
            {
                builder.OpenElement(9, "div");
                {
                    builder.AddAttribute(10, "class", ClassBuilder.Join(ClassNameProvider.Card_Footer, FooterClass));
                    builder.AddContent(11, FooterContent);
                }
                builder.CloseElement();
            }
        }
        builder.CloseElement();
    }
}
