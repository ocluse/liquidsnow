namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that collects <see cref="DateTimeOffset"/> values set to local time.
/// </summary>
public class DateTimePicker : TextBoxBase<DateTimeOffset?>
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
