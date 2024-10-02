namespace Ocluse.LiquidSnow.Utils;

/// <summary>
/// Utility methods for working with enums.
/// </summary>
public class EnumUtility
{
    /// <summary>
    /// Gets all the defined values of an <see cref="Enum"/> as an <see cref="IEnumerable{T}"/> list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> populated with the defined values of the enum <typeparamref name="T"/></returns>
#if NET
    [Obsolete("Use Enum.GetValues<T> instead starting .NET 5", true)]
#endif
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
