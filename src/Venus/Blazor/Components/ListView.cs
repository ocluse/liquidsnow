using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class ListView<T> : ItemsControl<T>
{
    protected override void BuildClass(List<string> classList)
    {
        base.BuildClass(classList);
        classList.Add("list");
    }
}
