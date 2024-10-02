using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A button component that displays a FluentUI icon.
/// </summary>
public class FluentIconButtonBase<T> : ButtonBase where T : FluentIcon
{
    /// <summary>
    /// Gets or sets the icon to display
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }


    /// <summary>
    /// Gets or sets the size of the icon in pixels.
    /// </summary>
    [Parameter]
    public int? Size { get; set; }

    ///<inheritdoc/>
    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("icon-button");
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<T>(2);
        builder.AddAttribute(3, nameof(FluentIcon.Size), Size ?? Resolver.DefaultButtonIconSize);
        builder.AddAttribute(4, nameof(FluentIcon.Icon), Icon);
        builder.CloseComponent();
    }
}

///<inheritdoc/>
public class FluentIconButton : FluentIconButtonBase<FluentIcon>
{
}

///<inheritdoc/>
public class Fluent12IconButton : FluentIconButtonBase<Fluent12Icon>
{
}

///<inheritdoc/>
public class Fluent16IconButton : FluentIconButtonBase<Fluent16Icon>
{
}

///<inheritdoc/>
public class Fluent20IconButton : FluentIconButtonBase<Fluent20Icon>
{
}

///<inheritdoc/>
public class Fluent28IconButton : FluentIconButtonBase<Fluent28Icon>
{
}

///<inheritdoc/>
public class Fluent32IconButton : FluentIconButtonBase<Fluent32Icon>
{
}

///<inheritdoc/>
public class Fluent48IconButton : FluentIconButtonBase<Fluent48Icon>
{
}
