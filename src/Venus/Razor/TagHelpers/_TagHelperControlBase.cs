namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public abstract class TagHelperControlBase : TagHelperElementBase
    {
        public int? Color { get; set; }

        public int? BackgroundColor { get; set; }

        protected IVenusResolver _resolver;

        public TagHelperControlBase(IVenusResolver resolver)
        {
            _resolver = resolver;
        }

        protected override void BuildStyle(StyleBuilder styleBuilder)
        {
            base.BuildStyle(styleBuilder);
            if (Color != null)
            {
                styleBuilder.Add($"color: {_resolver.ResolveColor(Color.Value)}");
            }
            if (BackgroundColor != null)
            {
                styleBuilder.Add($"background-color: {_resolver.ResolveColor(BackgroundColor.Value)}");
            }
        }
    }
}
