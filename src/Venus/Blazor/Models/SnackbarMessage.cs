namespace Ocluse.LiquidSnow.Venus.Blazor.Models
{
    public class SnackbarMessage
    {
        public required string Content { get; set; }
        public required int Status { get; set; }
        public SnackbarDuration Duration { get; set; }
    }
}
