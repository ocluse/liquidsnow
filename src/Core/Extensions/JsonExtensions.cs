using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Extensions;

/// <summary>
/// Provides extension methods for the System.Text.Json namespace.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Attempts to get a property from a JSON object, ignoring case.
    /// </summary>
    /// <returns>
    /// True if the property was found, with the value set to the value of the property.
    /// </returns>
    /// <param name="value">The value of the property, if found.</param>
    /// <param name="element">The JSON object to search.</param>
    /// <param name="propertyName">The name of the property to search for.</param>
    public static bool TryGetPropertyNoCase(this JsonElement element, string propertyName, out JsonElement value)
    {
        value = default;
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }
        return false;
    }
}
