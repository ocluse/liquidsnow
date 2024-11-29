using System.Globalization;
using System.Numerics;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that collects <see cref="INumber{TSelf}"/> values.
/// </summary>
public class NumberPicker<TValue> : TextBoxBase<TValue> where TValue : struct, INumber<TValue>
{
    /// <summary>
    /// Gets or sets the minimum value that can be picked.
    /// </summary>
    [Parameter]
    public TValue? Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum value that can be picked.
    /// </summary>
    [Parameter]
    public TValue? Max { get; set; }

    ///<inheritdoc/>
    protected override string InputType => "number";

    ///<inheritdoc/>
    protected override TValue GetValue(object? value)
    {
        string? s = value?.ToString();
        return string.IsNullOrWhiteSpace(s) ? Min ?? default : ParseValue(s) ?? default;
    }

    private TValue? ParseValue(string? value)
    {
        if (TValue.TryParse(value, CultureInfo.CurrentCulture, out var t))
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
