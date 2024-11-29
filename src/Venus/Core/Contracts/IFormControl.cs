namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Defines interactable components that can be hosted in forms.
/// </summary>
public interface IFormControl
{
    /// <summary>
    /// Gets or sets whether the control is disabled and cannot be interacted with.
    /// </summary>
    bool Disabled { get; set; }
}
