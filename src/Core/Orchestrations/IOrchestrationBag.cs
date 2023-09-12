using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// A bag of data that is passed between steps in an orchestration.
    /// </summary>
    public interface IOrchestrationBag : IReadOnlyDictionary<string, object?>
    {
        /// <summary>
        /// Gets a value from the bag.
        /// </summary>
        [return: MaybeNull]
        T Get<T>(string key);

        /// <summary>
        /// Sets a value in the bag.
        /// </summary>
        void Set<T>(string key, [AllowNull] T value);
    }
}
