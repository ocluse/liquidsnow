using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Ocluse.LiquidSnow.Extensions;

/// <summary>
/// Extensions for the System.Collections.Generic namespace.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Checks if a string is null or empty.
    /// </summary>
    public static bool IsNotEmpty([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Checks if a string is null or whitespace.
    /// </summary>
    public static bool IsNotWhiteSpace([NotNullWhen(true)] this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Transform a string in Pascal case to the equivalent Kebab case.
    /// </summary>
    /// <remarks>
    /// By default, numbers are separated by a dash. For example, 'WhereIsMyMother1' becomes 'where-is-my-mother-1'.
    /// This behaviour can be controlled by the <paramref name="numberDashing"/> parameter.
    /// </remarks>
    /// <param name="value">The string to transform to kebab case</param>
    /// <param name="numberDashing">If true, adds a dash at the start of digits</param>
    /// <param name="cultureInfo">The culture to use, if unspecified, uses the invariant culture.</param>
    /// <returns>The current string in Kebab case.</returns>
    public static string PascalToKebabCase(this string value, bool numberDashing = true, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.InvariantCulture;

        if (value.Length == 0) return string.Empty;

        StringBuilder builder = new();

        for (var i = 0; i < value.Length; i++)
        {
            if (char.IsLower(value[i])) // if current char is already lowercase
            {
                builder.Append(value[i]);
            }
            else if (i == 0) // if current char is the first char
            {
                builder.Append(char.ToLower(value[i], cultureInfo));
            }
            else if (char.IsDigit(value[i]) && !char.IsDigit(value[i - 1])) // if current char is a number and the previous is not
            {
                if (numberDashing)
                {
                    builder.Append('-');
                }

                builder.Append(value[i]);
            }
            else if (char.IsDigit(value[i])) // if current char is a number and previous is
            {
                builder.Append(value[i]);
            }
            else if (char.IsLower(value[i - 1])) // if current char is upper and previous char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLower(value[i], cultureInfo));
            }
            else if (i + 1 == value.Length || char.IsUpper(value[i + 1])) // if current char is upper and next char doesn't exist or is upper
            {
                builder.Append(char.ToLower(value[i], cultureInfo));
            }
            else // if current char is upper and next char is lower
            {
                builder.Append('-');
                builder.Append(char.ToLower(value[i], cultureInfo));
            }
        }
        return builder.ToString();
    }

    /// <summary>
    /// Transform a Pascal/Sentence/Title case string to Snake case'
    /// </summary>
    /// <param name="value">The string to transform to snake case</param>
    /// <param name="numberDashing">IF true, adds a dash at the start of digits</param>
    /// <remarks>
    /// By default, numbers are separated by an underscore, for example 'Hello World 1' or 'HelloWorld1' becomes 'hello_world_1'.
    /// This behaviour can be controlled by the <paramref name="numberDashing"/> parameter.
    /// </remarks>
    /// <returns>The input string in snake case</returns>
    public static string ToSnakeCase(this string value, bool numberDashing = true)
    {
        if (value.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length + Math.Min(2, value.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var currentIndex = 0; currentIndex < value.Length; currentIndex++)
        {
            var currentChar = value[currentIndex];
            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        previousCategory == UnicodeCategory.DecimalDigitNumber && currentCategory != UnicodeCategory.DecimalDigitNumber ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber &&
                        previousCategory != null &&
                        currentIndex > 0 &&
                        currentIndex + 1 < value.Length &&
                        char.IsLower(value[currentIndex + 1])
                        )
                    {
                        if (currentCategory == UnicodeCategory.DecimalDigitNumber && !numberDashing)
                        {
                            break;
                        }
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar, CultureInfo.InvariantCulture);
                    break;

                case UnicodeCategory.LowercaseLetter:

                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }
                    break;

                default:
                    if (previousCategory != null)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }
                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Adds spaces to a string that is in PascalCase.
    /// </summary>
    /// <param name="value">The string to which spaces are to be added</param>
    /// <param name="preserveAcronyms">If true, spaces are not added between continuous strings of uppercase characters</param>
    /// <remarks>
    /// <para>
    /// For example, 'WhereIsMyMotherSKL' becomes 'Where Is My Mother SKL'.
    /// </para>
    /// <para>
    /// Taken from https://stackoverflow.com/a/272929
    /// </para>
    /// </remarks>
    public static string AddSpacesToPascalCaseString(this string value, bool preserveAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        StringBuilder newText = new(value.Length * 2);
        newText.Append(value[0]);
        for (int i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]))
                if (value[i - 1] != ' ' && !char.IsUpper(value[i - 1]) ||
                    preserveAcronyms && char.IsUpper(value[i - 1]) &&
                     i < value.Length - 1 && !char.IsUpper(value[i + 1]))
                    newText.Append(' ');
            newText.Append(value[i]);
        }
        return newText.ToString();
    }

    /// <summary>
    /// Converts an uppercase/lowercase string to the Sentence case.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For example, 'hello world. WHERE IS MY MOTHER' becomes 'Hello world. Where is my mother'
    /// </para>
    /// <para>
    /// Solution sourced from https://stackoverflow.com/a/3141467
    /// </para>
    /// </remarks>
    /// <returns></returns>
    public static string ToSentenceCase(this string value, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.InvariantCulture;

        // start by converting entire string to lower case
        var lowerCase = value.ToLower(cultureInfo);

        // matches the first sentence of a string, as well as subsequent sentences
        var r = MatchFirstSentence();

        // MatchEvaluator delegate defines replacement of sentence starts to uppercase
        var result = r.Replace(lowerCase, s => s.Value.ToUpper(cultureInfo));

        return result;
    }

    /// <summary>
    /// Converts an uppercase/lowercase string to the Title case using the specified culture.
    /// </summary>
    /// <remarks>
    /// For example, 'WHERE IS MY MOTHER' becomes 'Where Is My Mother'
    /// </remarks>
    /// <param name="value">The string</param>
    /// <param name="cultureInfo">The culture to use, if unspecified, uses the invariant culture.</param>
    /// <returns></returns>
    public static string ToTitleCase(this string value, CultureInfo? cultureInfo = null)
    {
        cultureInfo ??= CultureInfo.InvariantCulture;

        TextInfo textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(value.ToLower(cultureInfo));
    }

    /// <summary>
    /// Converts a string to a URL encoded string.
    /// </summary>
    public static string ToUrlEncoded(this string value)
    {
        return Uri.EscapeDataString(value);
    }

    /// <summary>
    /// Converts a URL encoded string to a normal string.
    /// </summary>
    public static string ToUrlDecoded(this string value)
    {
        return Uri.UnescapeDataString(value);
    }

    /// <summary>
    /// Removes occurrences of the specified string from the start of the target string.
    /// </summary>
    public static string TrimStart(this string target, string trimString, bool firstOccurrenceOnly = false, StringComparison stringComparison = default)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        while (target.StartsWith(trimString, stringComparison))
        {
            target = target[trimString.Length..];

            if (firstOccurrenceOnly)
            {
                return target;
            }
        }

        return target;
    }

   
    /// <summary>
    /// Removes occurrences of the specified string from the end of the target string.
    /// </summary>
    public static string TrimEnd(this string target, string trimString, bool firstOccurrenceOnly = false, StringComparison stringComparison = default)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        while (target.EndsWith(trimString, stringComparison))
        {
            target = target[..^trimString.Length];

            if (firstOccurrenceOnly)
            {
                return target;
            }
        }

        return target;
    }

    /// <summary>
    /// Removes occurrences of the specified string from the start and end of the target string.
    /// </summary>
    public static string Trim(this string target, string trimString, bool firstOccurrenceOnly = false, StringComparison stringComparison = default)
    {
        return target.TrimStart(trimString, firstOccurrenceOnly).TrimEnd(trimString, firstOccurrenceOnly, stringComparison);
    }

    /// <summary>
    /// An extension method to convert a string to a Uri.
    /// </summary>
    public static Uri ToUri(this string value, UriKind uriKind = UriKind.RelativeOrAbsolute)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        }
        return new Uri(value, uriKind);
    }

    [GeneratedRegex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture)]
    private static partial Regex MatchFirstSentence();
}