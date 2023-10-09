namespace Ocluse.LiquidSnow.Venus.Blazor.Services
{
    public interface IBlazorResolver
    {
        int DefaultPageSize { get; }

        Type ResolveContainerStateToRenderType(int containerState);
        
        string ResolveSnackbarStatusToIcon(int status);
        
        int ResolveSnackbarDurationToMilliseconds(SnackbarDuration duration);
        
        int ResolveSnackbarStatusToColor(int status);

        string ResolveSnackbarStatusToClass(int status);
    }
}
