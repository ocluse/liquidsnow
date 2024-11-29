namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays items in a list.
/// </summary>
public class ListView<T> : CollectionViewBase<T>
{
    ///<inheritdoc/>
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add(ClassNameProvider.ListView_Container);
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.ListView);
    }
}
