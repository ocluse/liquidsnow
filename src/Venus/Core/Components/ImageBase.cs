using Microsoft.AspNetCore.Components.Rendering;

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
    /// Returns the value to set as the source of the image.
    /// </summary>
    protected virtual string? GetSource() => Src;

    /// <summary>
    /// Gets the default fallback source if the main source cannot be loaded.
    /// </summary>
    protected virtual string GetDefaultFallbackSrc() => Resolver.DefaultImageFallbackSrc;

    /// <summary>
    /// Returns the height of the image.
    /// </summary>
    protected abstract double GetHeight();

    /// <summary>
    /// Returns the width of the image.
    /// </summary>
    protected abstract double GetWidth();

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.Avatar);
    }

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

        if (src != null)
        {
            string fallbackSrc = FallbackSrc ?? GetDefaultFallbackSrc();
            attributes.Add("src", src);
            attributes.Add("onerror", $"this.src ='{fallbackSrc}';this.onerror=''");
        }

        string height = GetHeight().ToCssUnitValue(Unit ?? Resolver.DefaultImageSizeUnit);
        string width = GetWidth().ToCssUnitValue(Unit ?? Resolver.DefaultImageSizeUnit);
        
        attributes.Add("height", height);
        attributes.Add("width", width);

        if (Alt != null)
        {
            attributes.Add("alt", Alt);
        }
    }
}
