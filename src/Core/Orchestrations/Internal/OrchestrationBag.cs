using System.Collections.Generic;

namespace Ocluse.LiquidSnow.Orchestrations.Internal
{
    internal class OrchestrationBag : Dictionary<string, object?>, IOrchestrationBag
    {
        public T? Get<T>(string key)
        {
            if (ContainsKey(key))
            {
                return (T?)this[key];
            }
            else
            {
                return default;
            }
        }

        public void Set<T>(string key, T? value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }
}
