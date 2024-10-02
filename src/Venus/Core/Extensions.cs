using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Validations;
using Ocluse.LiquidSnow.Venus.Contracts;
using Ocluse.LiquidSnow.Venus.Services;
using System.Drawing;
using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ocluse.LiquidSnow.Venus;

/// <summary>
/// Adds extensions to various types
/// </summary>
public static class Extensions
{
    #region Dialog

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(Type, string?, bool, bool, Dictionary{string, object?}?)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string? dialogHeader, bool allowDismiss, bool showClose, Dictionary<string, object?>? parameters)
    {
        Type type = typeof(T);
        return await dialogService.ShowDialogAsync(type, dialogHeader, allowDismiss, showClose, parameters);
    }

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(Type, string?, bool, bool, Dictionary{string, object?}?)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string dialogHeader)
    {
        return await dialogService.ShowDialogAsync<T>(dialogHeader, false, true, null);
    }

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(Type, string?, bool, bool, Dictionary{string, object?}?)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string dialogHeader, Dictionary<string, object?> parameters)
    {
        return await dialogService.ShowDialogAsync<T>(dialogHeader, false, true, parameters);
    }

    ///<inheritdoc cref="IDialogService.ShowDialogAsync(Type, string?, bool, bool, Dictionary{string, object?}?)"/>
    public static async Task<DialogResult> ShowDialogAsync<T>(this IDialogService dialogService, string dialogHeader, bool allowDismiss, bool showClose)
    {
        return await dialogService.ShowDialogAsync<T>(dialogHeader, allowDismiss, showClose, null);
    }

    #endregion

    #region BuilderBase
    /// <summary>
    /// Adds a item to the builder.
    /// </summary>
    public static T Add<T>(this T builder, string? itemName) where T : BuilderBase
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
    public static T AddIf<T>(this T builder, bool condition, string? itemName) where T : BuilderBase
    {
        if (condition)
        {
            builder.Add(itemName);
        }
        return builder;
    }

    /// <summary>
    /// Adds a item to the builder if the condition is true, otherwise adds the elseClassName.
    /// </summary>
    public static T AddIfElse<T>(this T builder, bool condition, string? itemName, string? elseItemName) where T : BuilderBase
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
    public static T AddIf<T>(this T builder, bool condition, Func<string?> itemName) where T : BuilderBase
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
    public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, Func<string?> elseItemName) where T : BuilderBase
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
    public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, string? elseItemName) where T : BuilderBase
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
    public static T AddIfElse<T>(this T builder, bool condition, string? itemName, Func<string?> elseItemName) where T : BuilderBase
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
    public static T AddAll<T>(this T builder, IEnumerable<string?> itemNames) where T : BuilderBase
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

    /// <summary>
    /// Returns a string that will be displayed for the provided value.
    /// </summary>
    public static string? GetDisplayMemberValue<T>(this T? value, Func<T?, string>? displayMemberFunc, string? displayMemberPath)
    {
        if (displayMemberFunc != null)
        {
            return displayMemberFunc(value);
        }
        if (value == null)
        {
            return null;
        }
        if (displayMemberPath == null)
        {
            return value.ToString();
        }
        var property = value.GetType().GetProperty(displayMemberPath);
        if (property == null)
        {
            return null;
        }
        return property.GetValue(value)?.ToString();
    }

    /// <summary>
    /// Returns the HTML attribute key for the provided update trigger.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static string ToHtmlAttributeKey(this UpdateTrigger updateTrigger)
    {
        return updateTrigger switch
        {
            UpdateTrigger.OnChange => "onchange",
            UpdateTrigger.OnInput => "oninput",
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