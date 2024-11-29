namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that collects a <see cref="TimeOnly"/> value.
/// </summary>
public class TimePicker : TextBoxBase<TimeOnly?>
{
    ///<inheritdoc/>
    protected override string InputType => "time";

    ///<inheritdoc/>
    protected override TimeOnly? GetValue(object? value)
    {
        string? val = value?.ToString();

        return string.IsNullOrEmpty(val) ? null : TimeOnly.Parse(val);
    }

    ///<inheritdoc/>
    protected override object? GetInputDisplayValue(TimeOnly? value)
    {
        return value?.ToString("HH:mm");
    }
}
