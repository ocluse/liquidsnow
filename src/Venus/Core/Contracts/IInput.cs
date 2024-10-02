namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that accept user input.
/// </summary>
public interface IInput
{
    /// <summary>
    /// Gets or sets whether the input is disabled and cannot be interacted with.
    /// </summary>
    bool Disabled { get; set; }
}
