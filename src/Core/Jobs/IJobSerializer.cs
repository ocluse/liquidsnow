namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Serializes jobs into durable payloads and resolves their stable type names.
/// </summary>
public interface IJobSerializer
{
    /// <summary>
    /// Serializes a job using its runtime type.
    /// </summary>
    SerializedJob Serialize(IJob job);

    /// <summary>
    /// Deserializes a previously serialized job.
    /// </summary>
    IJob Deserialize(string typeName, ReadOnlyMemory<byte> payload);
}

/// <summary>
/// A serialized job payload and its stable type name.
/// </summary>
public sealed record SerializedJob(string TypeName, byte[] Payload);
