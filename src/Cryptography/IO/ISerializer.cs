namespace Ocluse.LiquidSnow.Cryptography.IO
{
    /// <summary>
    /// Utility for serializing and deserializing objects
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialize an object to a stream
        /// </summary>
        public Task SerializeAsync<T>(T data, Stream destinationStream) where T : class;

        /// <summary>
        /// Deserialize an object from a stream
        /// </summary>
        public Task<T?> DeserializeAsync<T>(Stream sourceStream) where T : class;
    }
}
