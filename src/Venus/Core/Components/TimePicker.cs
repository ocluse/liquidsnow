namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows the user to select a time value.
/// </summary>
public class TimePicker : InputControlBase<TimeOnly?>
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
