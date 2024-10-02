using Ocluse.LiquidSnow.Cryptography.IO.Internals;

namespace Ocluse.LiquidSnow.Cryptography.IO;

/// <summary>
/// Adds extension methods to the <see cref="ICryptoFile"/> and <see cref="ICryptoContainer"/> interfaces
/// </summary>
public static class Extensions
{
    private static readonly InternalSerializer _serializer = new();
    
    /// <summary>
    /// Serializes and encrypts an object, writing the resulting data to the file
    /// </summary>
    public static async Task SerializeAsync<T>(this ICryptoFile file, T o, ISerializer? serializer = null) where T : class
    {
        serializer ??= _serializer;

        using MemoryStream msData = new();

        await serializer.SerializeAsync(o, msData).ConfigureAwait(false);

        msData.Position = 0;

        await file.WriteAsync(msData).ConfigureAwait(false);
    }

    /// <summary>
    /// Decrypts and deserializes the contents of the file, returning the resultant object
    /// </summary>
    public static async Task<T?> DeserializeAsync<T>(this ICryptoFile file, ISerializer? serializer=  null) where T : class
    {
        serializer ??= _serializer;

        using MemoryStream msData = new();

        await file.ReadAsync(msData).ConfigureAwait(false);
        
        msData.Position = 0;

        return await serializer.DeserializeAsync<T>(msData).ConfigureAwait(false);
    }

    /// <summary>
    /// Serializes an object and adds it to the container.
    /// </summary>
    public static async Task AddAsync<T>(this ICryptoContainer container, string name, T o, bool overwrite = false, ISerializer? serializer = null) where T : class
    {
        serializer ??= _serializer;

        using MemoryStream msData = new();

        await serializer.SerializeAsync(o, msData).ConfigureAwait(false);

        msData.Position = 0;

        await container.AddStreamAsync(name, msData, overwrite).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads an object from the container and deserializes it.
    /// </summary>
    public static async Task<T?> GetAsync<T>(this ICryptoContainer container, string name, ISerializer? serializer = null) where T : class
    {
        serializer ??= _serializer;

        using MemoryStream msData = new();

        await container.GetStreamAsync(name, msData).ConfigureAwait(false);

        msData.Position = 0;

        return await serializer.DeserializeAsync<T>(msData).ConfigureAwait(false);
    }
}
