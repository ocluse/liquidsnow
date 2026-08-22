using System.Text.Json;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed class JsonJobSerializer : IJobSerializer
{
    private readonly Dictionary<Type, string> _namesByType = [];
    private readonly Dictionary<string, Type> _typesByName = new(StringComparer.Ordinal);
    private readonly JsonSerializerOptions _options;

    public JsonJobSerializer(IEnumerable<JobTypeRegistration> registrations, JobsOptions options)
    {
        _options = options.SerializerOptions;

        foreach (JobTypeRegistration registration in registrations)
        {
            if (_typesByName.TryGetValue(registration.TypeName, out Type? existingType) && existingType != registration.JobType)
            {
                throw new InvalidOperationException(
                    $"The durable job type name '{registration.TypeName}' is registered for both " +
                    $"'{existingType.FullName}' and '{registration.JobType.FullName}'.");
            }

            _typesByName[registration.TypeName] = registration.JobType;
            _namesByType[registration.JobType] = registration.TypeName;
        }
    }

    public SerializedJob Serialize(IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        Type jobType = job.GetType();

        if (!_namesByType.TryGetValue(jobType, out string? typeName))
        {
            throw new InvalidOperationException(
                $"Job type '{jobType.FullName}' has not been registered for durable serialization. " +
                "Register it with JobsBuilder.AddJob<T>(typeName).");
        }

        return new SerializedJob(typeName, JsonSerializer.SerializeToUtf8Bytes(job, jobType, _options));
    }

    public IJob Deserialize(string typeName, ReadOnlyMemory<byte> payload)
    {
        if (!_typesByName.TryGetValue(typeName, out Type? jobType))
        {
            throw new InvalidOperationException(
                $"The durable job type name '{typeName}' is not registered in this application.");
        }

        object? job = JsonSerializer.Deserialize(payload.Span, jobType, _options);
        return job as IJob
            ?? throw new InvalidOperationException($"The payload for '{typeName}' did not deserialize to an IJob.");
    }
}
