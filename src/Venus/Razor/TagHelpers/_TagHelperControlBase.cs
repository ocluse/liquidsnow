using Ocluse.LiquidSnow.Venus.Contracts;

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

        protected override void BuildStyle(List<string> styleList)
        {
            base.BuildStyle(styleList);
            if (Color != null)
            {
                styleList.Add($"color: {_resolver.ResolveColor(Color.Value)}");
            }
            if (BackgroundColor != null)
            {
                styleList.Add($"background-color: {_resolver.ResolveColor(BackgroundColor.Value)}");
            }
        }
    }
}
