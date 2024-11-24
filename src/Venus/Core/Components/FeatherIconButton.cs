using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders a <see cref="FeatherIcon"/> inside a clickable.
/// </summary>
public class FeatherIconButton : ClickableBase, ISvgIcon
{
    ///<inheritdoc/>
    [Parameter]
    public string? Icon { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? Size { get; set; }

    ///<inheritdoc/>
    public CssUnit? Unit { get; set; }

    ///<inheritdoc cref="FeatherIcon.StrokeWidth"/>
    [Parameter]
    public int? StrokeWidth { get; set; }

    ///<inheritdoc cref="FeatherIcon.StrokeLineCap"/>
    [Parameter]
    public StrokeLineCap? StrokeLineCap { get; set; }

    ///<inheritdoc cref="FeatherIcon.StrokeLineJoin"/>
    [Parameter]
    public StrokeLineJoin? StrokeLineJoin { get; set; }

    ///<inheritdoc/>
    protected override void BuildControlClass(ClassBuilder classBuilder)
    {
        base.BuildControlClass(classBuilder);
        classBuilder.Add(ClassNameProvider.FeatherIconButton);
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<FeatherIcon>(1);
        {
            builder.AddAttribute(2, nameof(FeatherIcon.Icon), Icon);
            builder.AddAttribute(3, nameof(FeatherIcon.Size), Size ?? Resolver.DefaultButtonIconSize);
            builder.AddAttribute(4, nameof(FeatherIcon.Unit), Unit);
            builder.AddAttribute(5, nameof(FeatherIcon.StrokeWidth), StrokeWidth ?? Resolver.IconStrokeWidth);
            builder.AddAttribute(6, nameof(FeatherIcon.StrokeLineCap), StrokeLineCap);
            builder.AddAttribute(7, nameof(FeatherIcon.StrokeLineJoin), StrokeLineJoin);
        }
        builder.CloseComponent();
    }
}
