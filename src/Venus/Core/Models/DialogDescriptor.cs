namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// Defines properties that are used to construct a dialog.
/// </summary>
public record DialogDescriptor
{
    /// <summary>
    /// Gets or initializes the type of component to render on the dialog's header.
    /// </summary>
    public Type? HeaderContentType { get; init; }

    /// <summary>
    /// Gets or initializes the parameters applied on the header component.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object?>>? HeaderParameters { get; init; }

    /// <summary>
    /// Gets or initializes the type of component to render on the dialog's body.
    /// </summary>
    public required Type ChildContentType { get; init; }

    /// <summary>
    /// Gets or initializes the parameters applied on the body component.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object?>>? ContentParameters { get; init; }

    /// <summary>
    /// Gets or initializes the type of component to render on the dialog's footer.
    /// </summary>
    public Type? FooterContentType { get; init; }

    /// <summary>
    /// Gets or initializes the parameters applied on the footer component.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object?>>? FooterParameters { get; init; }
}
