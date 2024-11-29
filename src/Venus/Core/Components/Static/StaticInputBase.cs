using Microsoft.AspNetCore.Components.Forms;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// The base class for statically rendered input components.
/// </summary>
public abstract class StaticInputBase<TValue> : Microsoft.AspNetCore.Components.Forms.InputBase<TValue>, IInputComponent
{
    /// <summary>
    /// Gets or sets the header for the component
    /// </summary>
    [Parameter]
    public string? Header { get; set; }

    /// <summary>
    /// Gets or sets the ID for the element
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets a value determining whether the control is disabled.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the a class that is applied when the control is disabled.
    /// </summary>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets the ID applied to the component's input.
    /// </summary>
    protected string EffectiveId => Id ?? NameAttributeValue;

    /// <summary>
    /// Gets or sets a value that determines whether a readonly attribute is applied to the input.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    ///<inheritdoc/>
    public string? ReadOnlyClass { get; set; }

    ///<inheritdoc/>
    public RenderFragment? PrefixContent { get; set; }

    ///<inheritdoc/>
    public RenderFragment? SuffixContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply when the value of the component is not valid.
    /// </summary>
    [Parameter]
    public string? ErrorClass { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input has a value that is not null.
    /// </summary>
    [Parameter]
    public string? HasValueClass { get; set; }

    ///<inheritdoc/>
    public RenderFragment<ValidationResult?>? ValidationContent { get; set; }

    ///<inheritdoc/>
    public string? Name { get; set; }

    ///<inheritdoc/>
    public string AppliedName => NameAttributeValue;

    ///<inheritdoc/>
    [Inject]
    public IVenusResolver Resolver { get; private set; } = default!;

    ///<inheritdoc/>
    [Inject]
    public IClassNameProvider ClassNameProvider { get; private set; } = default!;

    ///<inheritdoc/>
    public string? Padding { get; set; }

    ///<inheritdoc/>
    public CssUnit? SpacingUnit { get; set; }

    ///<inheritdoc/>
    public string? Margin { get; set; }

    ///<inheritdoc/>
    public string? Style { get; set; }

    ///<inheritdoc/>
    public string? Title { get; set; }

    /// <summary>
    /// Allows inheriting components to add custom styles to the element in the supplied <see cref="StyleBuilder"/>.
    /// </summary>
    protected virtual void BuildStyle(StyleBuilder builder) { }

    /// <summary>
    /// Allows inheriting components to add component-specific attributes.
    /// </summary>
    protected virtual void BuildAttributes(IDictionary<string, object> attributes) { }

    private void BuildClass(ClassBuilder builder)
    {
        BuildInputClass(builder);

        var validation = GetValidationResult();

        bool hasValue = Value is string stringVal ? stringVal.IsNotEmpty() : Value != null;

        builder.AddIf(validation?.IsValid == false, ClassNameProvider.InputError, ErrorClass)
            .AddIf(Disabled, ClassNameProvider.InputDisabled, DisabledClass)
            .AddIf(ReadOnly, ClassNameProvider.InputReadOnly, ReadOnlyClass)
            .AddIf(hasValue, ClassNameProvider.InputHasValue, HasValueClass);
    }

    /// <summary>
    /// Adds CSS classes to be applied to the input.
    /// </summary>
    protected virtual void BuildInputClass(ClassBuilder builder)
    {

    }

    /// <summary>
    /// Returns the CSS class to be applied to the component.
    /// </summary>
    protected string GetClass()
    {
        ClassBuilder builder = new();

        BuildClass(builder);

        builder.Add(Class);

        return builder.Build();

    }

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

    /// <inheritdoc cref="IInputComponent.GetValidationClass"/>
    protected virtual string? GetValidationClass()
    {
        var validation = GetValidationResult();

        return new ClassBuilder()
            .AddIf(validation?.IsValid == true, ClassNameProvider.ValidationSuccess)
            .AddIf(validation?.IsValid == false, ClassNameProvider.ValidationError)
            .Build();
    }

    string? IInputComponent.GetValidationClass()
    {
        return GetValidationClass();
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

    ///<inheritdoc/>
    public ValidationResult? GetValidationResult()
    {
        var validationMessages = EditContext.GetValidationMessages(FieldIdentifier);
        if (validationMessages.Any())
        {
            return new ValidationResult(false, string.Join('\n', validationMessages));
        }
        else
        {
            return null;
        }
    }
}
