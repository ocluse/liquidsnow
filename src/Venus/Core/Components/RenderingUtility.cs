using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// Contains utility methods to render an <see cref="IFieldComponent"/> for consistency.
/// </summary>
public static class RenderingUtility
{
    /// <summary>
    /// Renders the header content of the supplied <see cref="IFieldComponent"/> to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    public static void BuildFieldHeader(RenderTreeBuilder builder, IFieldComponent field)
    {
        //Header
        if (field.Header != null || field.HeaderContent != null)
        {
            string? headerClass = ClassBuilder.Join(field.ClassNameProvider.TextBox_Header, field.HeaderClass);

            if (field.HeaderContent != null)
            {
                builder.OpenElement(3, "div");
                {
                    builder.AddAttribute(4, "class", headerClass);
                    builder.AddContent(5, field.HeaderContent(field.AppliedName));
                }
                builder.CloseElement();
            }
            else
            {
                builder.OpenElement(6, "label");
                {
                    builder.AddAttribute(7, "class", headerClass);

                    if(field.Id != null)
                    {
                        builder.AddAttribute(8, "for", field.AppliedName);
                    }

                    builder.AddContent(9, field.Header);
                }
                builder.CloseElement();
            }
        }
    }

    /// <summary>
    /// Renders the supplied <see cref="IFieldComponent"/> to the supplied <see cref="RenderTreeBuilder"/>
    /// </summary>
    public static void BuildField(RenderTreeBuilder builder, IFieldComponent field)
    {
        var headerStyle = field.HeaderStyle ?? field.Resolver.DefaultFieldHeaderStyle;

        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, field.GetAttributes());

            //Static header:
            if (headerStyle == FieldHeaderStyle.Static)
            {
                builder.OpenRegion(3);
                {
                    BuildFieldHeader(builder, field);
                }
                builder.CloseRegion();
            }

            //The input content
            builder.OpenElement(4, "div");
            {
                builder.AddAttribute(5, "class", ClassBuilder.Join(field.ClassNameProvider.TextBox_Content, field.ContentClass));

                if (field.PrefixContent != null)
                {
                    builder.OpenElement(6, "div");
                    {
                        builder.AddAttribute(7, "class", ClassBuilder.Join(field.ClassNameProvider.TextBox_Prefix, field.PrefixClass));
                        builder.AddContent(8, field.PrefixContent);
                    }
                    builder.CloseElement();
                }

                builder.OpenRegion(9);
                {
                    field.BuildInput(builder);
                }
                builder.CloseRegion();

                //Floating header
                if (headerStyle == FieldHeaderStyle.Floating)
                {
                    builder.OpenRegion(10);
                    {
                        BuildFieldHeader(builder, field);
                    }
                    builder.CloseElement();
                }

                if (field.SuffixContent != null)
                {
                    builder.OpenElement(11, "div");
                    {
                        builder.AddAttribute(12, "class", ClassBuilder.Join(field.ClassNameProvider.TextBox_Suffix, field.SuffixClass));
                        builder.AddContent(13, field.SuffixContent);
                    }
                    builder.CloseElement();
                }
            }
            builder.CloseElement();

            var validation = field.GetValidationResult();

            //Validation message
            if (field.ValidationContent != null)
            {
                builder.OpenElement(14, "div");
                {
                    builder.AddAttribute(15, "class", field.GetValidationClass());
                    builder.AddContent(16, field.ValidationContent(validation));
                }
                builder.CloseElement();

            }
            else if (validation != null && validation.Message.IsNotEmpty())
            {
                builder.OpenElement(17, "label");
                {
                    builder.AddAttribute(18, "class", field.GetValidationClass());
                    builder.AddAttribute(19, "role", "alert");
                    builder.AddContent(20, validation.Message);
                }
                builder.CloseElement();
            }

            if (field is IAuxiliaryContentFieldComponent auxiliaryContentField)
            {
                builder.OpenRegion(21);
                {
                    auxiliaryContentField.BuildAuxiliaryContent(builder);
                }
                builder.CloseRegion();
            }
        }
        builder.CloseElement();
    }
}
