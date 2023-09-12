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

        protected override void BuildStyle(List<string> styleList)
        {
            base.BuildStyle(styleList);
            if (Color != null)
            {
                styleList.Add($"color: {VenusResolver.ResolveColor(Color.Value)}");
            }
            if (BackgroundColor != null)
            {
                styleList.Add($"background-color: {VenusResolver.ResolveColor(BackgroundColor.Value)}");
            }
        }
    }
}
