﻿using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A base class for components displaying images.
/// </summary>
public abstract class ImageBase : ControlBase
{
    /// <summary>
    /// Gets or sets the source of the image.
    /// </summary>
    [Parameter]
    public string? Src { get; set; }

    /// <summary>
    /// Gets or sets the unit of the size of the image.
    /// </summary>
    [Parameter]
    public CssUnit? Unit { get; set; }

    /// <summary>
    /// Gets or sets the alt text of the image.
    /// </summary>
    [Parameter]
    public string? Alt { get; set; }

    /// <summary>
    /// Gets or sets the fallback image source when the main source cannot be loaded.
    /// </summary>
    [Parameter]
    public string? FallbackSrc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use the fallback source when the main source is empty.
    /// </summary>
    [Parameter]
    public bool? UseFallbackForEmptySrc { get; set; }

    /// <summary>
    /// Returns the value to set as the source of the image.
    /// </summary>
    protected virtual string? GetSource() => Src;

    /// <summary>
    /// Gets the default fallback source if the main source cannot be loaded.
    /// </summary>
    protected virtual string GetDefaultFallbackSrc() => Resolver.DefaultImageFallbackSrc;

    /// <summary>
    /// Gets the default value for <see cref="UseFallbackForEmptySrc"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual bool GetDefaultUseFallbackForEmptySrc() => Resolver.DefaultUseFallbackForEmptyImageSrc;

    /// <summary>
    /// Returns the height of the image.
    /// </summary>
    protected abstract double? GetHeight();

    /// <summary>
    /// Returns the width of the image.
    /// </summary>
    protected abstract double? GetWidth();

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "img");
        {
            builder.AddMultipleAttributes(2, GetAttributes());
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);

        string? src = GetSource();
        string fallbackSrc = FallbackSrc ?? GetDefaultFallbackSrc();
        if (src.IsNotEmpty())
        {
            
            attributes.Add("src", src);
            attributes.Add("onerror", $"this.src ='{fallbackSrc}';this.onerror=''");
        }
        else if(UseFallbackForEmptySrc ?? GetDefaultUseFallbackForEmptySrc())
        {
            attributes.Add("src", fallbackSrc);
        }

        CssUnit unit = Unit ?? Resolver.DefaultImageSizeUnit;

        double? height = GetHeight();

        if (height.HasValue)
        {
            string heightAttribute = height.Value.ToCssUnitValue(unit);
            attributes.Add("height", heightAttribute);
        }

        double? width = GetWidth();
        
        if (width.HasValue)
        {
            string widthAttribute = width.Value.ToCssUnitValue(unit);
            attributes.Add("width", widthAttribute);
        }
       

        if (Alt != null)
        {
            attributes.Add("alt", Alt);
        }
    }
}
