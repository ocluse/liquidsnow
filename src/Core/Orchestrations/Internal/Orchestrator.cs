using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Orchestrations.Internal;

internal class Orchestrator(IServiceProvider serviceProvider, OrchestrationDescriptorCache descriptorCache) : IOrchestrator
{
    private async Task<TResult> ExecuteAsync<T, TResult>(T value, OrchestrationDescriptor descriptor, CancellationToken cancellationToken = default)
        where T : IOrchestration<TResult>
    {
        List<IOrchestrationStep<T, TResult>> steps = [.. serviceProvider
            .GetServices(descriptor.StepType)
            .Cast<IOrchestrationStep<T, TResult>>()
            .OrderBy(x => x.Order)];

        //ensure the orders do not have duplicates:
        if (steps.Select(x => x.Order).Distinct().Count() != steps.Count)
        {
            throw new InvalidOperationException("Duplicate orders found in the orchestration steps");
        }

        if (steps.Count == 0)
        {
            throw new InvalidOperationException($"No steps found for the orchestration");
        }

        var preprocessor = serviceProvider.GetService<IOrchestrationPreprocessor<T, TResult>>();

        var data = new OrchestrationData<T>(value);

        int startingIndex = 0;

        if (preprocessor != null)
        {
            IOrchestrationStepResult result = await preprocessor.ExecuteAsync(data, cancellationToken);

            if (result is IFinalOrchestrationResult<TResult> skip)
            {
                startingIndex = steps.Count;
            }

            data.AddResult(result);

            if (result.GoToOrder != null)
            {
                startingIndex = steps.FindIndex(x => x.Order == result.GoToOrder.Value);

                if (startingIndex == -1)
                {
                    throw new InvalidOperationException($"The orchestration step with order {result.GoToOrder.Value} was not found.");
                }
            }
        }

        for (int i = startingIndex; i < steps.Count;)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IOrchestrationStep<T, TResult> step = steps[i];

            bool canExecute;

            if (step is IConditionalOrchestrationStep<T, TResult> conditionalStep)
            {
                canExecute = await conditionalStep.CanExecute(data, cancellationToken);
            }
            else
            {
                canExecute = true;
            }

            int? goToOrder = null;

            if (canExecute)
            {
                IOrchestrationStepResult result = await step.ExecuteAsync(data, cancellationToken);

                data.AddResult(result);

                if (result is IFinalOrchestrationResult<TResult> skip)
                {
                    break;
                }

                goToOrder = result.GoToOrder;
            }

            if (goToOrder.HasValue)
            {
                i = steps.FindIndex(x => x.Order == goToOrder.Value);

                if (i == -1)
                {
                    throw new InvalidOperationException($"The orchestration step with order {goToOrder.Value} was not found.");
                }
            }
            else
            {
                i++;
            }
        }

        var postprocessor = serviceProvider.GetService<IOrchestrationPostprocessor<T, TResult>>();

        if (postprocessor != null)
        {
            return await postprocessor.ExecuteAsync(data, cancellationToken);
        }
        else
        {
            if (descriptor.IsUnit)
            {
                return default!;
            }
            else if (data.Results.Count > 1 && data.Results[^1] is IFinalOrchestrationResult<TResult> finalResult)
            {
                return finalResult.Data; 
            }
            else
            {
                throw new InvalidOperationException($"The orchestration produced no results after completion.");
            }
        }
    }

    public Task<T> ExecuteAsync<T>(IOrchestration<T> value, CancellationToken cancellationToken = default)
    {
        Type orchestrationType = value.GetType();

        OrchestrationDescriptor descriptor = descriptorCache.GetDescriptor(orchestrationType, typeof(T));

        return (Task<T>)descriptor.OrchestratorExecuteMethodInfo.Invoke(this, [value, descriptor, cancellationToken])!;
    }

    public async Task<TResult> ExecuteAsync<T, TResult>(T orchestration, CancellationToken cancellationToken = default)
        where T : IOrchestration<TResult>
    {
        OrchestrationDescriptor descriptor = descriptorCache.GetDescriptor(typeof(T), typeof(TResult));

        return await ExecuteAsync<T, TResult>(orchestration, descriptor, cancellationToken);
    }
}
