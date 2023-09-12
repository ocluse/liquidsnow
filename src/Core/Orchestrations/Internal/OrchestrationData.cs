using System.Collections.Generic;

namespace Ocluse.LiquidSnow.Orchestrations.Internal
{
    internal class OrchestrationData<T> : IOrchestrationData<T>
    {
        private readonly List<IOrchestrationStepResult> _results;

        public IReadOnlyList<IOrchestrationStepResult> Results => _results;

        public IOrchestrationBag Bag { get; }

        public T Orchestration { get; }


        public OrchestrationData(T value)
        {
            _results = new List<IOrchestrationStepResult>();
            Bag = new OrchestrationBag();
            Orchestration = value;
        }

        public void Advance(IOrchestrationStepResult result)
        {
            _results.Add(result);
        }
    }
}
