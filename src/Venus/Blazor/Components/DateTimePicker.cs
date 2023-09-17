namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class DateTimePicker : InputControlBase<DateTimeOffset?>
    {
        protected override string InputType { get; } = "datetime-local";

        protected override DateTimeOffset? GetValue(object? value)
        {
            string? val = value?.ToString();

            return string.IsNullOrEmpty(val) ? null : DateTimeOffset.Parse(val);
        }

        protected override object? GetInputDisplayValue(DateTimeOffset? value)
        {
            return value?.ToString("yyyy-MM-ddTHH:mm");
        }
    }
}
