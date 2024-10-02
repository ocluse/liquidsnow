namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component for selecting <see cref="DateOnly"/> values.
/// </summary>
public class DatePicker : InputControlBase<DateOnly?>
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
