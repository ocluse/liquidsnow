namespace Ocluse.LiquidSnow.Venus.Blazor.Contracts
{
    public interface ISnackbarItemHandle
    {
        string Message { get; }
        int Status { get; }
        SnackbarDuration Duration { get; }
        void Close();
        
    }
}
