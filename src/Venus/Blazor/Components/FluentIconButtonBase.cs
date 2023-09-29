using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class FluentIconButtonBase<T> : ButtonBase where T : FluentIcon
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public int Size { get; set; } = DefaultSize.Size18;

    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("icon-button");
    }
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<T>(2);
        builder.AddAttribute(3, nameof(FluentIcon.Size), Size);
        builder.AddAttribute(4, nameof(FluentIcon.Icon), Icon);
        builder.CloseComponent();
    }
}

public class FluentIconButton : FluentIconButtonBase<FluentIcon>
{
}

public class Fluent12IconButton : FluentIconButtonBase<Fluent12Icon>
{
}

public class Fluent16IconButton : FluentIconButtonBase<Fluent16Icon>
{
}

public class Fluent20IconButton : FluentIconButtonBase<Fluent20Icon>
{
}

public class Fluent28IconButton : FluentIconButtonBase<Fluent28Icon>
{
}

public class Fluent32IconButton : FluentIconButtonBase<Fluent32Icon>
{
}

public class Fluent48IconButton : FluentIconButtonBase<Fluent48Icon>
{
}
