﻿using Ocluse.LiquidSnow.Venus.Services;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A base class for all Venus controls.
/// </summary>
public class ControlBase : ElementBase
{
    /// <summary>
    /// Gets or sets the color of the control.
    /// </summary>
    [Parameter]
    public int? Color { get; set; }

    /// <summary>
    /// Gets or sets the background color of the control.
    /// </summary>
    [Parameter]
    public int? BackgroundColor { get; set; }

    /// <summary>
    /// The Venus resolver.
    /// </summary>
    [Inject]
    public required IVenusResolver Resolver { get; set; }

    ///<inheritdoc/>
    protected override void BuildStyle(StyleBuilder styleBuilder)
    {
        base.BuildStyle(styleBuilder);
        if (Color != null)
        {
            styleBuilder.Add($"color: {Resolver.ResolveColor(Color.Value)}");
        }
        if (BackgroundColor != null)
        {
            styleBuilder.Add($"background-color: {Resolver.ResolveColor(BackgroundColor.Value)}");
        }
    }
}
