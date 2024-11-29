using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays a loading spinner.
/// </summary>
public class LoadingSpinner : ControlBase
{
    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.LoadingSpinner);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        {
            builder.AddMultipleAttributes(1, GetAttributes());
        }
        builder.CloseElement();
    }
}
