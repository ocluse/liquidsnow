using System.Text;

namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Utility class for building a string of CSS classes.
/// </summary>
public class ClassBuilder : BuilderBase
{
    /// <summary>
    /// Returns a well formatted css class string represented by all the classes in the builder.
    /// </summary>
    public override string Build()
    {
        return string.Join(" ", Items);
    }

    /// <summary>
    /// A convenience method to join multiple classes together.
    /// </summary>
    public static string? Join(params string?[] classes)
    {
        StringBuilder sb = new();
        foreach (string? c in classes)
        {
            if (!string.IsNullOrWhiteSpace(c))
            {
                sb.Append(c);
                sb.Append(' ');
            }
        }

        return sb.ToString().Trim();
    }
}
