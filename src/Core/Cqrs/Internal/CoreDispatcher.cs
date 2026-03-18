namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class CoreDispatcher(IEnumerable<ICqrsDispatchContributor> contributors)
{
    private readonly Dictionary<DispatchKey, DispatchDescriptor> _descriptors = contributors
        .SelectMany(x => x.Descriptors)
        .GroupBy(x => new DispatchKey(x.Kind, x.RequestType, x.ResultType))
        .ToDictionary(
            x => x.Key,
            x => x.Single());

    private readonly Dictionary<DispatchKey, DispatchDescriptor> _resolvedCache = new();
    private readonly object _resolvedCacheLock = new();

    public async Task<TCommandResult> DispatchCommandAsync<TCommandResult>(ICommand<TCommandResult> command, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        DispatchDescriptor descriptor = ResolveDescriptor(DispatchKind.Command, command.GetType(), typeof(TCommandResult));
        object result = await descriptor.ExecuteAsync(command, serviceProvider, cancellationToken);
        return (TCommandResult)result;
    }

    public Task<TCommandResult> DispatchCommandAsync<TCommand, TCommandResult>(TCommand command, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        where TCommand : ICommand<TCommandResult>
    {
        return CqrsDispatchExecutor.ExecuteCommandAsync<TCommand, TCommandResult>(command, serviceProvider, cancellationToken);
    }

    public async Task<TQueryResult> DispatchQueryAsync<TQueryResult>(IQuery<TQueryResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        DispatchDescriptor descriptor = ResolveDescriptor(DispatchKind.Query, query.GetType(), typeof(TQueryResult));
        object result = await descriptor.ExecuteAsync(query, serviceProvider, cancellationToken);
        return (TQueryResult)result;
    }

    public Task<TQueryResult> DispatchQueryAsync<TQuery, TQueryResult>(TQuery query, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        where TQuery : IQuery<TQueryResult>
    {
        return CqrsDispatchExecutor.ExecuteQueryAsync<TQuery, TQueryResult>(query, serviceProvider, cancellationToken);
    }

    private DispatchDescriptor ResolveDescriptor(DispatchKind kind, Type requestType, Type resultType)
    {
        var key = new DispatchKey(kind, requestType, resultType);

        lock (_resolvedCacheLock)
        {
            if (_resolvedCache.TryGetValue(key, out DispatchDescriptor? cached))
            {
                return cached;
            }
        }

        if (_descriptors.TryGetValue(key, out DispatchDescriptor? descriptor))
        {
            lock (_resolvedCacheLock)
            {
                _resolvedCache[key] = descriptor;
            }

            return descriptor;
        }

        List<DispatchDescriptor> candidates = _descriptors.Values
            .Where(d => d.Kind == kind && d.ResultType == resultType && d.RequestType.IsAssignableFrom(requestType))
            .ToList();

        if (candidates.Count == 0)
        {
            throw new InvalidOperationException($"No handler found for {kind} request type '{requestType.FullName}' and result type '{resultType.FullName}'.");
        }

        int highestScore = candidates.Max(x => ComputeMatchScore(requestType, x.RequestType));

        List<DispatchDescriptor> bestMatches = candidates
            .Where(x => ComputeMatchScore(requestType, x.RequestType) == highestScore)
            .ToList();

        if (bestMatches.Count > 1)
        {
            string matches = string.Join(", ",
                bestMatches.Select(x => $"{x.RequestType.FullName} -> {x.ResultType.FullName}"));

            throw new InvalidOperationException(
                $"Ambiguous handlers found for {kind} request type '{requestType.FullName}' and result type '{resultType.FullName}'. Matches: {matches}.");
        }

        descriptor = bestMatches[0];

        lock (_resolvedCacheLock)
        {
            _resolvedCache[key] = descriptor;
        }

        return descriptor;
    }

    private static int ComputeMatchScore(Type runtimeType, Type candidateType)
    {
        if (runtimeType == candidateType)
        {
            return 10_000;
        }

        if (candidateType.IsInterface)
        {
            Type[] interfaces = runtimeType.GetInterfaces();
            int index = Array.IndexOf(interfaces, candidateType);
            if (index >= 0)
            {
                return 2_000 - index;
            }

            return int.MinValue;
        }

        int distance = 0;
        Type? current = runtimeType;
        while (current != null)
        {
            if (current == candidateType)
            {
                return 5_000 - distance;
            }

            current = current.BaseType;
            distance++;
        }

        return int.MinValue;
    }

}
