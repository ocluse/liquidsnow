using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for elements that are rendered as 'fields', including inputs and dropdowns.
/// </summary>
public abstract class FieldBase<TValue> : InputBase<TValue>
{
    /// <summary>
    /// Gets or sets the CSS class applied to the input's content area.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the prefix content containing div.
    /// </summary>
    [Parameter]
    public string? PrefixClass { get; set; }

    /// <summary>
    /// Gets or sets the style of the header.
    /// </summary>
    [Parameter]
    public FieldHeaderStyle? HeaderStyle { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the suffix content containing div.
    /// </summary>
    [Parameter]
    public string? SuffixClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the header of the textbox.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the inner input element. 
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the validation label of the input.
    /// </summary>
    [Parameter]
    public string? ValidationLabelClass { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the header of the input.
    /// </summary>
    [Parameter]
    public RenderFragment<string>? HeaderContent { get; set; }

    /// <summary>
    /// Gets a value that determines whether the builder should build auxiliary content.
    /// </summary>
    protected abstract bool HasAuxiliaryContent { get; }

    private void BuildHeader(RenderTreeBuilder builder)
    {
        //Header
        if (Header != null || HeaderContent != null)
        {
            string? headerClass = ClassBuilder.Join(ClassNameProvider.TextBox_Header, HeaderClass);

            if (HeaderContent != null)
            {
                builder.OpenElement(3, "div");
                {
                    builder.AddAttribute(4, "class", headerClass);
                    builder.AddContent(5, HeaderContent(AppliedName));
                }
                builder.CloseElement();
            }
            else
            {
                builder.OpenElement(6, "label");
                {
                    builder.AddAttribute(7, "class", headerClass);
                    builder.AddAttribute(8, "for", AppliedName);
                    builder.AddContent(9, Header);
                }
                builder.CloseElement();
            }
        }
    }

    ///<inheritdoc/>
    protected override sealed void BuildRenderTree(RenderTreeBuilder builder)
    {
        var headerStyle = HeaderStyle ?? Resolver.DefaultFieldHeaderStyle;

        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            //Static header:
            if(headerStyle == FieldHeaderStyle.Static)
            {
                builder.OpenRegion(3);
                {
                    BuildHeader(builder);
                }
                builder.CloseRegion();
            }

            //The input content
            builder.OpenElement(4, "div");
            {
                builder.AddAttribute(5, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Content, ContentClass));

                if (PrefixContent != null)
                {
                    builder.OpenElement(6, "div");
                    {
                        builder.AddAttribute(7, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Prefix, PrefixClass));
                        builder.AddContent(8, PrefixContent);
                    }
                    builder.CloseElement();
                }

                builder.OpenRegion(9);
                {
                    BuildInput(builder);
                }
                builder.CloseRegion();

                //Floating header
                if(headerStyle == FieldHeaderStyle.Floating)
                {
                    builder.OpenRegion(10);
                    {
                        BuildHeader(builder);
                    }
                    builder.CloseElement();
                }

                if (SuffixContent != null)
                {
                    builder.OpenElement(11, "div");
                    {
                        builder.AddAttribute(12, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Suffix, SuffixClass));
                        builder.AddContent(13, SuffixContent);
                    }
                    builder.CloseElement();
                }
            }
            builder.CloseElement();

            //Validation message
            if (ValidationContent != null)
            {
                builder.OpenElement(14, "div");
                {
                    builder.AddAttribute(15, "class", GetValidationClass());
                    builder.AddContent(16, ValidationContent(Validation));
                }
                builder.CloseElement();

            }
            else if (Validation != null && !string.IsNullOrEmpty(Validation.Message))
            {
                builder.OpenElement(17, "label");
                {
                    builder.AddAttribute(18, "class", GetValidationClass());
                    builder.AddAttribute(19, "role", "alert");
                    builder.AddContent(20, Validation.Message);
                }
                builder.CloseElement();
            }

            if (HasAuxiliaryContent)
            {
                builder.OpenRegion(21);
                {
                    BuildAuxiliaryContent(builder);
                }
                builder.CloseRegion();
            }
        }
        builder.CloseElement();
    }

    /// <summary>
    /// Build the actual input element.
    /// </summary>
    protected abstract void BuildInput(RenderTreeBuilder builder);

    /// <summary>
    /// Renders auxiliary content, just before the field is closed.
    /// </summary>
    protected virtual void BuildAuxiliaryContent(RenderTreeBuilder builder) { }

    ///<inheritdoc/>
    protected override string GetValidationClass()
    {
        string baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, ClassNameProvider.TextBox_ValidationLabel, ValidationLabelClass) ?? baseClass;
    }
}
