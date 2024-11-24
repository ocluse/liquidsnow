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

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

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

            //The input content
            builder.OpenElement(10, "div");
            {
                builder.AddAttribute(11, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Content, ContentClass));

                if (PrefixContent != null)
                {
                    builder.OpenElement(12, "div");
                    {
                        builder.AddAttribute(13, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Prefix, PrefixClass));
                        builder.AddContent(14, PrefixContent);
                    }
                    builder.CloseElement();
                }

                builder.OpenRegion(15);
                {
                    BuildInput(builder);
                }
                builder.CloseRegion();

                if (SuffixContent != null)
                {
                    builder.OpenElement(16, "div");
                    {
                        builder.AddAttribute(17, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Suffix, SuffixClass));
                        builder.AddContent(18, SuffixContent);
                    }
                    builder.CloseElement();
                }
            }
            builder.CloseElement();

            //Validation message
            if (ValidationContent != null)
            {
                builder.OpenElement(19, "div");
                {
                    builder.AddAttribute(20, "class", GetValidationClass());
                    builder.AddContent(21, ValidationContent(Validation));
                }
                builder.CloseElement();

            }
            else if (Validation != null && !string.IsNullOrEmpty(Validation.Message))
            {
                builder.OpenElement(22, "label");
                {
                    builder.AddAttribute(23, "class", GetValidationClass());
                    builder.AddAttribute(24, "role", "alert");
                    builder.AddContent(25, Validation.Message);
                }
                builder.CloseElement();
            }

            if (HasAuxiliaryContent)
            {
                builder.OpenRegion(26);
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
    protected abstract void BuildAuxiliaryContent(RenderTreeBuilder builder);

    ///<inheritdoc/>
    protected override string GetValidationClass()
    {
        string baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, ClassNameProvider.TextBox_ValidationLabel, ValidationLabelClass) ?? baseClass;
    }
}
