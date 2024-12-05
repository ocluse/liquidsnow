using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// The base class for static components that are rendered as 'fields', including inputs and dropdowns.
/// </summary>
public abstract class StaticFieldBase<TValue> : StaticInputBase<TValue>, IFieldComponent
{
    /// <summary>
    /// Gets or sets the ID for the element
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets a placeholder for the component
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public RenderFragment<string>? HeaderContent { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public string? ContentClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public string? PrefixClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public string? SuffixClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the input.
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public FieldHeaderStyle? HeaderStyle { get; set; }

    /// <summary>
    /// Gets or sets the notification strategy for when the value of the input changes.
    /// </summary>
    [Parameter]
    public UpdateTrigger UpdateTrigger { get; set; }

    ///<inheritdoc/>
    public bool? AlwaysRenderValidationLabel { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        RenderingUtility.BuildField(builder, this);
    }

    ///<inheritdoc/>
    protected abstract void BuildInput(RenderTreeBuilder builder);

    void IFieldComponent.BuildInput(RenderTreeBuilder builder)
    {
        BuildInput(builder);
    }

    ///<inheritdoc/>
    protected override string? GetValidationClass()
    {
        string? baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, ClassNameProvider.Field_ValidationLabel, ValidationLabelClass) ?? baseClass;
    }
}
