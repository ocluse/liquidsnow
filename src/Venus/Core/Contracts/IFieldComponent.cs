using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Contracts;

internal interface IAuxiliaryContentFieldComponent : IFieldComponent
{
    void BuildAuxiliaryContent(RenderTreeBuilder builder);
}

internal interface IFieldComponent : IComponent, IValidatable
{
    IVenusResolver Resolver { get; set; }
    
    IClassNameProvider ClassNameProvider { get; set; }
    
    string AppliedName { get; set; }

    FieldHeaderStyle? HeaderStyle { get; set; }

    string Header { get; set; }

    string? HeaderClass { get; set; }

    RenderFragment<string>? HeaderContent { get; set; }

    RenderFragment? PrefixContent { get; set; }

    RenderFragment? SuffixContent { get; set; }

    RenderFragment<ValidationResult?>? ValidationContent { get; set; }

    string? ContentClass { get; set; }
    
    string? PrefixClass { get; set; }
    
    string? SuffixClass { get; set; }
    
    Dictionary<string, object> GetAttributes();

    void BuildInput(RenderTreeBuilder builder);

    string? GetValidationClass();
}
