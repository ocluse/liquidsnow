using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A button component that renders a <see cref="FluentIcon"/> inside a clickable.
/// </summary>
public class FluentIconButtonBase<T> :  ClickableBase, ISvgIcon where T : FluentIcon
{
    ///<inheritdoc/>
    [Parameter]
    public string? Icon { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? Size { get; set; }

    ///<inheritdoc/>
    public CssUnit? Unit { get; set; }

    ///<inheritdoc/>
    protected override void BuildControlClass(ClassBuilder classBuilder)
    {
        base.BuildControlClass(classBuilder);
        classBuilder.Add(ClassNameProvider.FluentIconButton);
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<T>(1);
        builder.AddAttribute(2, nameof(FluentIcon.Size), Size ?? Resolver.DefaultButtonIconSize);
        builder.AddAttribute(3, nameof(FluentIcon.Icon), Icon);
        builder.AddAttribute(4, nameof(FluentIcon.Unit), Unit);
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
