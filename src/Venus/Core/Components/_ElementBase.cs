namespace Ocluse.LiquidSnow.Venus.Components;
/// <summary>  
/// Base class for components built around Venus design principles.  
/// </summary>  
public abstract class ElementBase : ComponentBase
{
    /// <summary>  
    /// Gets or sets the padding for the element.  
    /// </summary>  
    [Parameter]
    public string? Padding { get; set; }

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
    /// Allows derived classes to build custom styles for the element.
    /// </summary>
    /// <param name="builder">The <see cref="StyleBuilder"/> used to build the style.</param>
    protected virtual void BuildStyle(StyleBuilder builder) { }

    /// <summary>
    /// Allows derived classes to build custom classes for the element.
    /// </summary>
    /// <param name="builder">The <see cref="ClassBuilder"/> used to build the class.</param>
    protected virtual void BuildClass(ClassBuilder builder) { }

    /// <summary>
    /// Allows derived classes to add component-specific attributes.
    /// </summary>
    protected virtual void BuildAttributes(IDictionary<string, object> attributes) { }

    /// <summary>
    /// Gets the complete style string for the element.
    /// </summary>
    /// <returns>A string representing the style of the element.</returns>
    protected string GetStyle()
    {
        StyleBuilder styleBuilder = new();

        if (!string.IsNullOrEmpty(Margin))
        {
            styleBuilder.Add($"margin: {Margin.ParseThicknessValues()};");
        }

        if (!string.IsNullOrEmpty(Padding))
        {
            styleBuilder.Add($"padding: {Padding.ParseThicknessValues()};");
        }

        BuildStyle(styleBuilder);

        styleBuilder.Add(Style);

        return styleBuilder.Build();
    }

    /// <summary>
    /// Gets the applied CSS class.
    /// </summary>
    protected string GetClass()
    {
        ClassBuilder classBuilder = new();

        BuildClass(classBuilder);

        classBuilder.Add(Class);

        return classBuilder.Build();
    }

    /// <summary>
    /// Returns the attributes to be applied to the component, including the style, class and any other specified by the derived class
    /// </summary>
    protected Dictionary<string, object> GetAttributes()
    {
        Dictionary<string, object> attributes = new()
       {
           { "class", GetClass() },
           {"style", GetStyle() },
       };

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
