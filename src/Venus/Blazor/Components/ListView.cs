namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class ListView<T> : ItemsControl<T>
{
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add("list");
    }
}
