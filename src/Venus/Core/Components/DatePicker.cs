namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that collects <see cref="DateOnly"/> values.
/// </summary>
public class DatePicker : TextBoxBase<DateOnly?>
{
    ///<inheritdoc/>
    protected override string InputType => "date";

    ///<inheritdoc/>
    protected override DateOnly? GetValue(object? value)
    {
        string? val = value?.ToString();

        return string.IsNullOrEmpty(val) ? null : DateOnly.Parse(val);
    }

    ///<inheritdoc/>
    protected override object? GetInputDisplayValue(DateOnly? value)
    {
        return value?.ToString("yyyy-MM-dd");
    }
}
