using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for elements that are rendered as 'fields', including inputs and dropdowns.
/// </summary>
public abstract class FieldBase<TValue> : InputBase<TValue>, IFieldComponent
{
    ///<inheritdoc/>
    [Parameter]
    public string? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the placeholder to display when the input is empty.
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
    /// Gets or sets the CSS class applied to the validation label of the input.
    /// </summary>
    [Parameter]
    public string? ValidationLabelClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the inner input element. 
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public FieldHeaderStyle? HeaderStyle { get; set; }

    ///<inheritdoc/>
    protected override sealed void BuildRenderTree(RenderTreeBuilder builder)
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
