namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Utility class for building a string of CSS classes or styles.
/// </summary>
public abstract class CssBuilderBase
{
    /// <summary>
    /// The list of items in the builder.
    /// </summary>
    protected List<string> Items { get; }= [];

    /// <summary>
    /// Adds an item to the builder.
    /// </summary>
    public CssBuilderBase Add(string? item)
    {
        if (item != null)
        {
            Items.Add(item);
        }
        return this;
    }

    /// <summary>
    /// Adds a range of items to the builder.
    /// </summary>
    public CssBuilderBase AddRange(IEnumerable<string?> items)
    {
        foreach(string? item in items)
        {
            Add(item);
        }
        
        return this;
    }

    /// <summary>
    /// Returns a well formatted string represented by all the items in the builder.
    /// </summary>
    public abstract string Build();
}
