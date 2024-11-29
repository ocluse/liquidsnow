namespace Ocluse.LiquidSnow.Venus.Contracts;

internal interface IDialogHandle
{
    Task CloseAsync(bool? success, object? data);
}
