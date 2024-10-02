namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Utility class for building a string of CSS classes or styles.
/// </summary>
public abstract class BuilderBase
{
    /// <summary>
    /// The list of items in the builder.
    /// </summary>
    protected List<string> Items { get; }= [];

    /// <summary>
    /// Adds a class to the builder.
    /// </summary>
    public BuilderBase Add(string? className)
    {
        if (className != null)
        {
            Items.Add(className);
        }
        return this;
    }

    /// <summary>
    /// Returns a well formatted css class string represented by all the classes in the builder.
    /// </summary>
    public abstract string Build();
}
