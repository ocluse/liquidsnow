namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Defines a view that displays a collection of items.
/// </summary>
public interface ICollectionView<T>
{
    /// <summary>
    /// Gets the items that the view is displaying.
    /// </summary>
    IEnumerable<T>? Items { get; }

    /// <summary>
    /// Gets the template for rendering each item in the view.
    /// </summary>
    RenderFragment<T>? ItemTemplate { get; }

    /// <summary>
    /// Gets the function that returns the display member for the given item.
    /// </summary>
    Func<T?, string>? ToStringFunc { get; }
}

/// <summary>
/// Defines views that display data.
/// </summary>
public interface IDataView
{
    /// <summary>
    /// The state of the data view.
    /// </summary>
    int State { get; }

    /// <summary>
    /// A function to cause the view to reload its data.
    /// </summary>
    Task ReloadDataAsync();
}
