using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class FluentIconButton<T> : ButtonBase where T : FluentIcon
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public int Size { get; set; } = DefaultSize.Size24;

    protected override void BuildButtonClass(List<string> classList)
    {
        base.BuildButtonClass(classList);
        classList.Add("icon-button");
    }
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.OpenComponent<T>(2);
        builder.AddAttribute(3, nameof(FluentIcon.Size), Size);
        builder.AddAttribute(4, nameof(FluentIcon.Icon), Icon);
        builder.CloseComponent();
    }
}

public class FluentIconButton : FluentIconButton<FluentIcon>
{
}

public class Fluent12IconButton : FluentIconButton<Fluent12Icon>
{
}

public class Fluent16IconButton : FluentIconButton<Fluent16Icon>
{
}

public class Fluent20IconButton : FluentIconButton<Fluent20Icon>
{
}

public class Fluent28IconButton : FluentIconButton<Fluent28Icon>
{
}

public class Fluent32IconButton : FluentIconButton<Fluent32Icon>
{
}

public class Fluent48IconButton : FluentIconButton<Fluent48Icon>
{
}
