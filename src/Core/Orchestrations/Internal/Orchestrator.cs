using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Orchestrations.Internal
{
    internal class Orchestrator : IOrchestrator
    {
        private readonly IServiceProvider _serviceProvider;

        public Orchestrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private static T ReturnAsResult<T>(object? data)
        {
            if (data is T result)
            {
                return result;
            }
            throw new InvalidCastException($"The data returned by the orchestration is not of type {typeof(T).Name}");
        }

        private async Task<TResult> Execute<T, TResult>(T value, CancellationToken cancellationToken = default)
            where T : IOrchestration<TResult>
        {
            Type orchestrationType = value.GetType();

            Type[] typeArgs = [orchestrationType, typeof(TResult)];

            Type orchestrationStepType = typeof(IOrchestrationStep<,>).MakeGenericType(typeArgs);

            List<IOrchestrationStep<T, TResult>> steps = _serviceProvider
                .GetServices(orchestrationStepType)
                .Cast<IOrchestrationStep<T, TResult>>()
                .OrderBy(x => x.Order)
                .ToList();

            if (steps.Count == 0)
            {
                throw new InvalidOperationException($"No steps found for orchestration {orchestrationType.Name}");
            }

            var preliminaryStep = _serviceProvider.GetService<IPreliminaryOrchestrationStep<T, TResult>>();

            var data = new OrchestrationData<T>(value);

            int order = steps[0].Order;

            RequiredState? previousState = null;

            if (preliminaryStep != null)
            {
                IOrchestrationStepResult result = await preliminaryStep.Execute(data, cancellationToken);

                if (result is ISkipOrchestrationResult skip)
                {
                    return ReturnAsResult<TResult>(skip.Data);
                }

                data.Advance(result);

                previousState = result.IsSuccess ? RequiredState.Success : RequiredState.Failure;

                if (result.JumpToOrder != null)
                {
                    order = result.JumpToOrder.Value;
                }
            }

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                IOrchestrationStep<T, TResult> step = steps.FirstOrDefault(x => x.Order == order)
                    ?? throw new InvalidOperationException($"No step found with order {order}");

                bool canExecute = true;

                if (step is IStateDependentOrchestrationStep<T, TResult> stateDependentStep)
                {
                    canExecute = previousState == stateDependentStep.RequiredState;
                }

                if (canExecute)
                {
                    if (step is IConditionalOrchestrationStep<T, TResult> conditionalStep)
                    {
                        canExecute = await conditionalStep.CanExecute(data, cancellationToken);
                    }

                    int? jumpToOrder = null;

                    if (canExecute)
                    {
                        IOrchestrationStepResult result = await step.Execute(data, cancellationToken);
                        data.Advance(result);

                        if (result is ISkipOrchestrationResult skip)
                        {
                            return ReturnAsResult<TResult>(skip.Data);
                        }

                        jumpToOrder = result.JumpToOrder;
                        previousState = result.IsSuccess ? RequiredState.Success : RequiredState.Failure;
                    }

                    if (jumpToOrder != null)
                    {
                        order = jumpToOrder.Value;
                    }
                    else if (steps[^1] == step)
                    {
                        break;
                    }
                    else
                    {
                        // set order to the next step
                        order = steps[steps.IndexOf(step) + 1].Order;
                    }
                }
            }

            var finalStep = _serviceProvider.GetService<IFinalOrchestrationStep<T, TResult>>();

            if (finalStep != null)
            {
                return await finalStep.Execute(data, cancellationToken);
            }
            else
            {
                if (data.Results.Count == 0)
                {
                    throw new InvalidOperationException($"The orchestration {orchestrationType.Name} produced no results after completion.");
                }

                return ReturnAsResult<TResult>(data.Results[^1]);
            }
        }

        public Task<T> Execute<T>(IOrchestration<T> value, CancellationToken cancellationToken = default)
        {
            Type orchestrationType = value.GetType();

            Type[] typeArgs = [orchestrationType, typeof(T)];

            var methodInfo = GetType().GetMethod(nameof(Execute), BindingFlags.NonPublic | BindingFlags.Instance);

            var genericMethodInfo = methodInfo!.MakeGenericMethod(typeArgs);

            return (Task<T>)genericMethodInfo.Invoke(this, [value, cancellationToken])!;
        }
    }
}
