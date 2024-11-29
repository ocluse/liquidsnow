using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// The base class for statically rendered input components.
/// </summary>
public abstract class StaticInputBase<TValue> : Microsoft.AspNetCore.Components.Forms.InputBase<TValue>
{
    //protected FieldIdentifier _fieldIdentifier;

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

    //[CascadingParameter]
    //public required EditContext EditContext { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets a value indicating whether the component has validation errors.
    /// </summary>
    public bool HasErrors => GetValidationMessages().Any();

    /// <summary>
    /// Gets or sets the ID applied to the component's input.
    /// </summary>
    protected string EffectiveId => Id ?? NameAttributeValue;

    //public TValue? Value => EditContext.Model.GetPropValue<TValue>(_fieldIdentifier.FieldName);

    //protected override void OnParametersSet()
    //{
    //    base.OnParametersSet();

    //    if (Bind is not null)
    //    {
    //        _fieldIdentifier = FieldIdentifier.Create(Bind);
    //    }

    //}

    /// <summary>
    /// Returns the validation messages for the component.
    /// </summary>
    public IEnumerable<string> GetValidationMessages()
    {
        return EditContext.GetValidationMessages(FieldIdentifier);
    }

    private void BuildClass(ClassBuilder classBuilder)
    {
        BuildInputClass(classBuilder);

        if (HasErrors)
        {
            classBuilder.Add("error");
        }

        if (Disabled)
        {
            classBuilder.Add(DisabledClass ?? "disabled");
        }
    }

    /// <summary>
    /// Adds CSS classes to be applied to the input.
    /// </summary>
    protected virtual void BuildInputClass(ClassBuilder classBuilder)
    {

    }

    /// <summary>
    /// Returns the CSS class to be applied to the component.
    /// </summary>
    protected string GetClass()
    {
        ClassBuilder classBuilder = new();

        BuildClass(classBuilder);

        classBuilder.Add(Class);

        return classBuilder.Build();

    }
}
