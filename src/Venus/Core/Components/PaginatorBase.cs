using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;
using System.Text;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base component for pagination components.
/// </summary>
public abstract class PaginatorBase : ControlBase
{
    /// <summary>
    /// Gets or sets the content displayed in the pagination next button.
    /// </summary>
    [Parameter]
    public RenderFragment? NextContent { get; set; }

    /// <summary>
    /// Gets or sets the content displayed in the pagination previous button.
    /// </summary>
    [Parameter]
    public RenderFragment? PreviousContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class that is applied when a pagination navigation item is disabled.
    /// </summary>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class that is applied to the active pagination item.
    /// </summary>
    [Parameter]
    public string? ActiveItemClass { get; set; }


    /// <summary>
    /// Gets or sets the CSS class to be added to the pagination next button.
    /// </summary>
    [Parameter]
    public string? NextClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to be added to the pagination previous button.
    /// </summary>
    [Parameter]
    public string? PreviousClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the pagination should automatically reload 
    /// a cascading <see cref="IDataView"/> component when the state changes.
    /// </summary>
    [Parameter]
    public bool AutoReloadParent { get; set; } = true;

    [CascadingParameter]
    private IDataView? DataView { get; set; }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        classBuilder.Add(ClassNameProvider.Paginator);
        base.BuildClass(classBuilder);
    }

    /// <summary>
    /// Gets a CSS class builder that allows to build the CSS class for a pagination item.
    /// </summary>
    protected ClassBuilder GetItemClassBuilder()
    {
        ClassBuilder builder = new();
        builder.Add(ClassNameProvider.Paginator_Item);
        return builder;
    }

    /// <summary>
    /// Gets the CSS class applied to a pagination item.
    /// </summary>
    protected string GetItemClass(bool enabled, bool active, string? baseClass)
    {
        return GetItemClassBuilder()
            .AddIf(!enabled, DisabledClass ?? ClassNameProvider.ComponentDisabled)
            .AddIf(active, ActiveItemClass ?? ClassNameProvider.Paginator_ItemActive)
            .AddIf(baseClass != null, baseClass)
            .Build();
    }

    /// <summary>
    /// Gets a StringBuilder that allows to build the CSS class for a pagination item.
    /// </summary>
    protected StringBuilder GetPaginationItemBuilder(string itemClass, bool disabled)
    {
        StringBuilder sb = new();

        sb.Append(ClassNameProvider.Paginator_Item);

        if (itemClass != null)
        {
            sb.Append(' ');
            sb.Append(itemClass);
        }

        if (disabled)
        {
            sb.Append(' ');
            if (DisabledClass != null)
            {
                sb.Append(DisabledClass);
            }
            else
            {
                sb.Append(ClassNameProvider.ComponentDisabled);
            }
        }

        return sb;
    }

    /// <summary>
    /// Reloads a cascading <see cref="IDataView"/> component when the state changes.
    /// </summary>
    protected async Task ReloadDataViewAsync()
    {
        if (AutoReloadParent && DataView != null)
        {
            await DataView.ReloadDataAsync();
        }
    }

    /// <summary>
    /// Builds the inner content of a skip button to the render tree.
    /// </summary>
    protected void BuildSkipButtonContent(RenderTreeBuilder builder, bool isNext)
    {
        var content = isNext ? NextContent : PreviousContent;

        if (content != null)
        {
            builder.AddContent(1, content);
        }
        else
        {
            var icon = ComponentIcons.Get(Resolver.IconStyle, isNext ? ComponentIcon.ChevronRight : ComponentIcon.ChevronLeft);
            var componentType = Resolver.IconStyle == IconStyle.Feather ? typeof(FeatherIcon) : typeof(FluentIcon);

            builder.OpenComponent(2, componentType);
            {
                builder.AddAttribute(3, nameof(FeatherIcon.Icon), icon);
                builder.AddAttribute(4, nameof(FeatherIcon.Size), Resolver.DefaultButtonIconSize);
            }
            builder.CloseComponent();
        }
    }
}
