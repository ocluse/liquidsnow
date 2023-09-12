namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class DatePicker : InputControlBase<DateOnly?>
    {
        
        protected override DateOnly? GetValue(object? value)
        {
            string? val = value?.ToString();

            return string.IsNullOrEmpty(val) ? null : DateOnly.Parse(val);
        }

        protected override string GetInputType()
        {
            return "date";
        }

        protected override object? GetInputDisplayValue(DateOnly? value)
        {
            return value?.ToString("yyyy-MM-dd");
        }
    }
}
