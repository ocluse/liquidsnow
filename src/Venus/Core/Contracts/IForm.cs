namespace Ocluse.LiquidSnow.Venus.Contracts;
/// <summary>
/// A contract for components that host form inputs.
/// </summary>
public interface IForm
{
    /// <summary>
    /// Called by form inputs to register themselves with the container when they enter the render tree.
    /// </summary>
    void Register(IInput input);

    /// <summary>
    /// Called by form inputs to unregister themselves with the container when they leave the render tree.
    /// </summary>
    void Unregister(IInput input);
}
