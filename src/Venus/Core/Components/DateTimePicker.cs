namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows the user to pick a date and time.
/// </summary>
public class DateTimePicker : InputControlBase<DateTimeOffset?>
{
    ///<inheritdoc/>
    protected override string InputType => "datetime-local";

    ///<inheritdoc/>
    protected override DateTimeOffset? GetValue(object? value)
    {
        string? val = value?.ToString();

        return string.IsNullOrEmpty(val) ? null : DateTimeOffset.Parse(val);
    }

    ///<inheritdoc/>
    protected override object? GetInputDisplayValue(DateTimeOffset? value)
    {
        return value?.ToString("yyyy-MM-ddTHH:mm");
    }
}
