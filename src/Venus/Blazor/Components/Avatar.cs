using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class Avatar : ControlBase
    {
        [Parameter]
        public string Src { get; set; } = string.Empty;

        [Parameter]
        public string? UserId { get; set; }

        [Parameter]
        public int Size { get; set; } = DefaultSize.Size48;

        [Parameter]
        public string? SrcOnError { get; set; } = "/images/anonymous.svg";

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("avatar");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "img");

            string src =  string.IsNullOrEmpty(UserId) ? Src : VenusResolver.ResolveAvatarId(UserId);

            builder.AddAttribute(1, "src", src);
            builder.AddAttribute(2, "height", Size);
            builder.AddAttribute(2, "width", Size);
            builder.AddAttribute(3, "onerror", $"this.src ='{SrcOnError}';this.onerror=''");
            builder.AddAttribute(4, "class", GetClass());
            builder.CloseElement();
        }
    }
}
