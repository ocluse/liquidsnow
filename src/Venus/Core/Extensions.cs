using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Validations;
using Ocluse.LiquidSnow.Venus.Contracts;
using Ocluse.LiquidSnow.Venus.Services;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Adds extensions to various types
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Venus to the service collection, returning a builder that can be used to configure other Venus services.
    /// </summary>
    public static VenusServiceBuilder AddVenus(this IServiceCollection services)
    {
        VenusServiceBuilder builder = new(services);
        return builder.AddResolver<VenusResolver>();
    }

    /// <summary>
    /// Returns the string representation of the number in K (thousands), M (millions), B (billions), T (trillions) format
    /// </summary>
    /// <remarks>
    /// For example 1000 will be returned as 1K, 1000000 will be returned as 1M, 1000000000 will be returned as 1B, 1000000000000 will be returned as 1T
    /// </remarks>
    public static string ToKMB<T>(this T num, CultureInfo? cultureInfo = null) where T : INumber<T>
    {
        T trillion = T.Parse("999999999999", cultureInfo);
        T billion = T.Parse("999999999", cultureInfo);
        T million = T.Parse("999999", cultureInfo);
        T thousand = T.Parse("999", cultureInfo);

        if (num > trillion || num < -trillion)
        {
            return num.ToString("0,,,,.###T", cultureInfo);
        }

        else if (num > billion || num < -billion)
        {
            return num.ToString("0,,,.###B", cultureInfo);
        }
        else
        if (num > million || num < -million)
        {
            return num.ToString("0,,.##M", cultureInfo);
        }
        else
        if (num > thousand || num < -thousand)
        {
            return num.ToString("0,.#K", cultureInfo);
        }
        else
        {
            return num.ToString(format: null, formatProvider: cultureInfo);
        }
    }

    /// <summary>
    /// Validates that a date is not null
    /// </summary>
    public static Task<ValidationResult> DateNotNull(this CommonValidators validators, DateOnly? value)
    {
        if (value == null)
        {
            return Task.FromResult(ValidationResult.NotValid(validators.ValueRequiredMessage));
        }
        return Task.FromResult(ValidationResult.ValidResult);
    }

    /// <summary>
    /// Validates that a number is greater than zero
    /// </summary>
    public static Task<ValidationResult> NumberGreaterThanZero<T>(this CommonValidators validators, T value) where T : INumber<T>
    {
        if (value == null)
        {
            return Task.FromResult(ValidationResult.NotValid(validators.ValueRequiredMessage));
        }

        if (value <= T.Zero)
        {
            return Task.FromResult(ValidationResult.NotValid(validators.ValueMustBeGreaterThanZeroMessage));
        }

        return Task.FromResult(ValidationResult.ValidResult);
    }

    internal static string ToRem(this double value)
    {
        return $"{value / 2}rem";
    }

    internal static string ParseThicknessValues(this string value)
    {
        var values = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(double.Parse)
            .ToList();

        if (values.Count == 1)
        {
            return values[0].ToRem();
        }
        else if (values.Count == 2)
        {
            return $"{values[1].ToRem()} {values[0].ToRem()}";
        }
        else if (values.Count == 4)
        {
            return $"{values[1].ToRem()} {values[2].ToRem()} {values[3].ToRem()} {values[0].ToRem()} {values[1].ToRem()}";
        }
        else
        {
            throw new FormatException($"Cannot format {value}. Illegal number of elements");
        }
    }

    internal static IEnumerable<string> GetGridStyles(this IGrid grid)
    {
        List<string> styleList =
            [
                $"--grid-columns:{TranslateToGridTemplate(grid.Columns)}"
            ];

        //Lg:
        int lg = grid.ColumnsLg ?? grid.Columns;
        styleList.Add($"--grid-columns-lg: {TranslateToGridTemplate(lg)}");

        //Md:
        int md = grid.ColumnsMd ?? lg;
        styleList.Add($"--grid-columns-md: {TranslateToGridTemplate(md)}");

        //Sm:
        int sm = grid.ColumnsSm ?? md;
        styleList.Add($"--grid-columns-sm: {TranslateToGridTemplate(sm)}");

        //Xs:
        int xs = grid.ColumnsXs ?? sm;
        styleList.Add($"--grid-columns-xs: {TranslateToGridTemplate(xs)}");

        //Column Gap
        double columnGap = grid.ColumnGap ?? grid.Gap;
        styleList.Add($"--grid-column-gap: {columnGap / 2}rem");

        //Row Gap
        double rowGap = grid.RowGap ?? grid.Gap;
        styleList.Add($"--grid-row-gap: {rowGap / 2}rem;");

        return styleList;
    }

    private static string TranslateToGridTemplate(int columns)
    {
        StringBuilder sb = new();
        for (int i = 0; i < columns; i++)
        {
            sb.Append("1fr ");
        }
        return sb.ToString().Trim();
    }
}