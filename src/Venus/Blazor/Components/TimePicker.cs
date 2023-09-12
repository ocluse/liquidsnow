namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class TimePicker : InputControlBase<TimeOnly?>
    {
        protected override TimeOnly? GetValue(object? value)
        {
            string? val = value?.ToString();

            return string.IsNullOrEmpty(val) ? null : TimeOnly.Parse(val);
        }

        protected override object? GetInputDisplayValue(TimeOnly? value)
        {
            return value?.ToString("HH:mm");
        }
        protected override string GetInputType()
        {
            return "time";
        }
    }
}
