using System.Text.Json;

namespace Ocluse.LiquidSnow.Cryptography.IO.Internals
{
    internal class InternalSerializer : ISerializer
    {
        public async Task SerializeAsync<T>(T data, Stream destinationStream) where T : class
        {
            await JsonSerializer.SerializeAsync(destinationStream, data);
        }

        public async Task<T?> DeserializeAsync<T>(Stream sourceStream) where T : class
        {
            return await JsonSerializer.DeserializeAsync<T>(sourceStream);
        }
    }
}
