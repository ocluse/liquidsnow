namespace Ocluse.LiquidSnow.Orchestrations;

/// <summary>
/// Defines bag that can be used to share data between steps in an orchestration.
/// </summary>
public interface IOrchestrationBag : IReadOnlyDictionary<string, object?>
{
    /// <summary>
    /// Gets a value from the bag.
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in the bag.
    /// </summary>
    void Set<T>(string key, T? value);
}
