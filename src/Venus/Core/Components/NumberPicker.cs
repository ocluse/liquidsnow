using System.Globalization;
using System.Numerics;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows the user to pick a number.
/// </summary>
public class NumberPicker<T> : InputControlBase<T> where T : struct, INumber<T>
{
    /// <summary>
    /// The minimum value that can be picked.
    /// </summary>
    [Parameter]
    public T? Min { get; set; }

    /// <summary>
    /// The maximum value that can be picked.
    /// </summary>
    [Parameter]
    public T? Max { get; set; }

    ///<inheritdoc/>
    protected override string InputType => "number";

    ///<inheritdoc/>
    protected override T GetValue(object? value)
    {
        string? s = value?.ToString();
        return string.IsNullOrWhiteSpace(s) ? Min ?? default : ParseValue(s) ?? default;
    }

    private T? ParseValue(string? value)
    {
        if (T.TryParse(value, CultureInfo.CurrentCulture, out var t))
        {
            if (Min != null && t.CompareTo(Min) < 0)
            {
                return Min;
            }
            else if (Max != null && t.CompareTo(Max) > 0)
            {
                return Max;
            }
            else
            {
                return t;
            }
        }
        else
        {
            return Value;
        }
    }
}
