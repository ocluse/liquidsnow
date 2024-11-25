namespace Ocluse.LiquidSnow.Venus.Contracts;

internal interface IContainer
{
    RenderFragment EmptyTemplate { get; }

    RenderFragment LoadingTemplate { get; }

    RenderFragment<Exception?> ErrorTemplate { get; }

    RenderFragment UnauthorizedTemplate { get; }

    RenderFragment NotFoundTemplate { get; }
}
