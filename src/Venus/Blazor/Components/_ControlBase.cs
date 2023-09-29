namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ControlBase : ElementBase
    {
        [Parameter]
        public int? Color { get; set; }

        [Parameter]
        public int? BackgroundColor { get; set; }

        [Inject]
        public required IVenusResolver VenusResolver { get; set; }

        protected override void BuildStyle(StyleBuilder styleBuilder)
        {
            base.BuildStyle(styleBuilder);
            if (Color != null)
            {
                styleBuilder.Add($"color: {VenusResolver.ResolveColor(Color.Value)}");
            }
            if (BackgroundColor != null)
            {
                styleBuilder.Add($"background-color: {VenusResolver.ResolveColor(BackgroundColor.Value)}");
            }
        }
    }
}
