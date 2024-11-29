using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;
/// <summary>  
/// The base class for components that render html elements.
/// </summary>  
public abstract class ElementBase : VenusComponentBase, IElementComponent
{
    /// <summary>  
    /// Gets or sets the padding for the element.  
    /// </summary>  
    [Parameter]
    public string? Padding { get; set; }

    /// <summary>
    /// Gets or sets the unit for the spacing values.
    /// </summary>
    [Parameter]
    public CssUnit? SpacingUnit { get; set; }

    /// <summary>  
    /// Gets or sets the margin for the element.  
    /// </summary>  
    [Parameter]
    public string? Margin { get; set; }

    /// <summary>  
    /// Gets or sets the CSS class for the element.  
    /// </summary>  
    [Parameter]
    public string? Class { get; set; }

    /// <summary>  
    /// Gets or sets the inline style for the element.  
    /// </summary>  
    [Parameter]
    public string? Style { get; set; }

    /// <summary>  
    /// Gets or sets the title for the element.  
    /// </summary>  
    [Parameter]
    public string? Title { get; set; }

    /// <summary>  
    /// Gets or sets additional attributes for the element.  
    /// </summary>  
    [Parameter]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Allows inheriting components to add custom styles to the element in the supplied <see cref="StyleBuilder"/>.
    /// </summary>
    protected virtual void BuildStyle(StyleBuilder builder) { }

    /// <summary>
    /// Allows inheriting components to add custom classes for the element in the supplied <see cref="ClassBuilder"/>.
    /// </summary>
    protected virtual void BuildClass(ClassBuilder builder) { }

    /// <summary>
    /// Allows inheriting components to add component-specific attributes.
    /// </summary>
    protected virtual void BuildAttributes(IDictionary<string, object> attributes) { }

    /// <summary>
    /// Returns the complete style string for the element.
    /// </summary>
    protected string GetStyle()
    {
        StyleBuilder styleBuilder = new();

        if (!string.IsNullOrEmpty(Margin))
        {
            styleBuilder.Add($"margin: {Margin.ParseSpacingValues(SpacingUnit ?? Resolver.DefaultSpacingUnit)};");
        }

        if (!string.IsNullOrEmpty(Padding))
        {
            styleBuilder.Add($"padding: {Padding.ParseSpacingValues(SpacingUnit ?? Resolver.DefaultSpacingUnit)};");
        }

        BuildStyle(styleBuilder);

        styleBuilder.Add(Style);

        return styleBuilder.Build();
    }

    /// <summary>
    /// Returns the complete CSS class string for the element.
    /// </summary>
    protected string GetClass()
    {
        ClassBuilder classBuilder = new();

        BuildClass(classBuilder);

        classBuilder.Add(Class);

        return classBuilder.Build();
    }

    ///<inheritdoc/>
    public Dictionary<string, object> GetAttributes()
    {
        Dictionary<string, object> attributes = [];

        string? style = GetStyle();
        string? className = GetClass();

        if (style.IsNotWhiteSpace())
        {
            attributes.Add("style", style);
        }

        if (className.IsNotWhiteSpace())
        {
            attributes.Add("class", className);
        }

        if (!string.IsNullOrEmpty(Title))
        {
            attributes.Add("title", Title);
        }

        BuildAttributes(attributes);

        if (AdditionalAttributes != null)
        {
            foreach (var attribute in AdditionalAttributes)
            {
                attributes.Add(attribute.Key, attribute.Value);
            }
        }

        return attributes;
    }
}
