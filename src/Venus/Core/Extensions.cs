using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Venus.Components;
using Ocluse.LiquidSnow.Venus.Components.Internal;
using System.Numerics;
using System.Text;

namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Adds extensions to various types
/// </summary>
public static class Extensions
{
    #region Dialog

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(DialogDescriptor, CancellationToken)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string? title, bool showClose, Dictionary<string, object?>? parameters)
    {
        Type type = typeof(T);
        DialogDescriptor descriptor = new()
        {
            ContentParameters = parameters,
            ChildContentType = type,
            HeaderContentType = typeof(DialogHeader),
            HeaderParameters = new Dictionary<string, object?>()
            {
                {nameof(DialogHeader.Options), new DialogHeaderOptions(title, showClose) }
            },
            FooterContentType = null,
            FooterParameters = null,
        };

        return await dialogService.ShowDialogAsync(descriptor);
    }

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(DialogDescriptor, CancellationToken)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string title)
    {
        return await dialogService.ShowDialogAsync<T>(title, true, null);
    }

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(DialogDescriptor, CancellationToken)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string title, Dictionary<string, object?> parameters)
    {
        return await dialogService.ShowDialogAsync<T>(title, true, parameters);
    }
    #endregion

    #region BuilderBase
    /// <summary>
    /// Adds a item to the builder.
    /// </summary>
    public static T Add<T>(this T builder, string? itemName) where T : CssBuilderBase
    {
        if (itemName != null)
        {
            builder.Add(itemName);
        }
        return builder;
    }

    /// <summary>
    /// Adds a item to the builder if the condition is true.
    /// </summary>
    public static T AddIf<T>(this T builder, bool condition, params string?[] itemNames) where T : CssBuilderBase
    {
        if (condition && itemNames.Length > 0)
        {
            builder.AddRange(itemNames);
        }
        return builder;
    }

    /// <summary>
    /// Adds a item to the builder if the condition is true, otherwise adds the elseClassName.
    /// </summary>
    public static T AddIfElse<T>(this T builder, bool condition, string? itemName, string? elseItemName) where T : CssBuilderBase
    {
        if (condition)
        {
            builder.Add(itemName);
        }
        else
        {
            builder.Add(elseItemName);
        }
        return builder;
    }

    /// <summary>
    /// Adds the item returned by a function to the builder if the condition is true.
    /// </summary>
    public static T AddIf<T>(this T builder, bool condition, Func<string?> itemName) where T : CssBuilderBase
    {
        if (condition)
        {
            builder.Add(itemName());
        }
        return builder;
    }

    /// <summary>
    /// Adds one or the other of items depending on a condition.
    /// </summary>
    public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, Func<string?> elseItemName) where T : CssBuilderBase
    {
        if (condition)
        {
            builder.Add(itemName());
        }
        else
        {
            builder.Add(elseItemName());
        }
        return builder;
    }

    /// <inheritdoc cref="AddIfElse{T}(T, bool, Func{string?}, Func{string?})"/>
    public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, string? elseItemName) where T : CssBuilderBase
    {
        if (condition)
        {
            builder.Add(itemName());
        }
        else
        {
            builder.Add(elseItemName);
        }
        return builder;
    }

    /// <inheritdoc cref="AddIfElse{T}(T, bool, Func{string?}, Func{string?})"/>
    public static T AddIfElse<T>(this T builder, bool condition, string? itemName, Func<string?> elseItemName) where T : CssBuilderBase
    {
        if (condition)
        {
            builder.Add(itemName);
        }
        else
        {
            builder.Add(elseItemName());
        }
        return builder;
    }

    /// <summary>
    /// Adds all the items to the builder.
    /// </summary>
    public static T AddAll<T>(this T builder, IEnumerable<string?> itemNames) where T : CssBuilderBase
    {
        foreach (var itemName in itemNames)
        {
            builder.Add(itemName);
        }
        return builder;
    }
    #endregion

    #region Dependency Injection
    /// <summary>
    /// Adds Venus services to the collection, using the default resolver.
    /// </summary>
    public static IServiceCollection AddVenus(this IServiceCollection services)
    {
        return services.AddVenus<VenusResolver>();
    }

    /// <summary>
    /// Adds Venus to the service collection, returning a builder that can be used to configure other Venus services.
    /// </summary>
    public static IServiceCollection AddVenus<T>(this IServiceCollection services)
        where T : class, IVenusResolver
    {
        return services.AddSingleton<IDialogService, DialogService>()
            .AddSingleton<ISnackbarService, SnackbarService>()
            .AddSingleton<IVenusResolver, T>();
    }
    #endregion

    #region Resolver

    /// <summary>
    /// Returns the appropriate type of component to render for icons under the specified resolver.
    /// </summary>
    public static Type GetIconComponentType(this IVenusResolver resolver)
    {
        return resolver.IconStyle switch
        {
            IconStyle.Feather => typeof(FeatherIcon),
            IconStyle.Fluent => typeof(FluentIcon),
            _=> throw new NotImplementedException($"Unknown icon type: {resolver.IconStyle}")
        };
    }

    /// <summary>
    /// Returns the appropriate type of component to render for icon buttons under the specified resolver.
    /// </summary>
    public static Type GetIconButtonComponentType(this IVenusResolver resolver)
    {
        return resolver.IconStyle switch
        {
            IconStyle.Feather => typeof(FeatherIconButton),
            IconStyle.Fluent => typeof(FluentIconButton),
            _ => throw new NotImplementedException($"Unknown icon type: {resolver.IconStyle}")
        };
    }

    #endregion

    /// <summary>
    /// Returns the HTML attribute value for the provided stroke line cap.
    /// </summary>
    public static string ToHtmlAttributeValue(this StrokeLineCap strokeLineCap)
    {
        return strokeLineCap switch
        {
            StrokeLineCap.Butt => "butt",
            StrokeLineCap.Round => "round",
            StrokeLineCap.Square => "square",
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Returns the value suffixed with the appropriate CSS unit.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static string ToCssUnitValue<T>(this T value, CssUnit unit) where T : INumber<T>
    {
        return $"{value}{unit.ToCssValue()}";
    }

    /// <summary>
    /// Returns the HTML attribute value for the provided stroke line join.
    /// </summary>
    public static string ToHtmlAttributeValue(this StrokeLineJoin strokeLineJoin)
    {
        return strokeLineJoin switch
        {
            StrokeLineJoin.Arcs => "arcs",
            StrokeLineJoin.Bevel => "bevel",
            StrokeLineJoin.Round => "round",
            StrokeLineJoin.Miter => "miter",
            StrokeLineJoin.MiterClip => "miter-clip",
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Returns the HTML attribute value for the provided update trigger.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static string ToHtmlAttributeValue(this UpdateTrigger updateTrigger)
    {
        return updateTrigger switch
        {
            UpdateTrigger.OnChange => "onchange",
            UpdateTrigger.OnInput => "oninput",
            _ => throw new NotImplementedException()
        };
    }


    /// <summary>
    /// Returns the CSS unit suffix for the provided CSS unit.
    /// </summary>
    public static string ToCssValue(this CssUnit unit)
    {
        return unit switch
        {
            CssUnit.Em => "em",
            CssUnit.Rem => "rem",
            CssUnit.Px => "px",
            CssUnit.Percent => "%",
            CssUnit.Fr => "fr",
            CssUnit.VW => "vw",
            CssUnit.VH => "vh",
            CssUnit.VMin => "vmin",
            CssUnit.VMax => "vmax",
            CssUnit.DVH => "dvh",
            CssUnit.DVW => "dvw",
            _ => throw new NotImplementedException()
        };
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

    private static string ToLengthExpression(this string value)
    {
        if (double.TryParse(value, out double parsedValue))
        {
            return $"{parsedValue / 2}em";
        }
        else
        {
            return value;
        }
    }

    internal static string GetIconSize(this ISvgIcon icon, IVenusResolver resolver)
    {
        return (icon.Size ?? resolver.DefaultIconSize).ToCssUnitValue(icon.Unit ?? resolver.DefaultIconSizeUnit);
    }

    internal static string? GetDisplayValue<T>(this T? value, Func<T?, string>? displayMemberFunc)
    {
        if (displayMemberFunc != null)
        {
            return displayMemberFunc(value);
        }
        if (value == null)
        {
            return null;
        }
        return value.ToString();
    }

    internal static string ParseThicknessValues(this string value)
    {
        var values = value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(ToLengthExpression);

        int count = values.Count();

        if (count == 1 && count == 2 && count == 4)
        {
            return string.Join(' ', values);
        }
        else
        {
            throw new FormatException($"Cannot format {value}. Invalid number of elements provided");
        }
    }

    internal static IEnumerable<string> GetGridStyles(this IGrid grid, IVenusResolver resolver)
    {
        string gapSuffix = (grid.GapUnit ?? resolver.DefaultGapUnit).ToCssValue();

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
        styleList.Add($"--grid-column-gap: {columnGap / 2}{gapSuffix}");

        //Row Gap
        double rowGap = grid.RowGap ?? grid.Gap;
        styleList.Add($"--grid-row-gap: {rowGap / 2}{gapSuffix};");

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