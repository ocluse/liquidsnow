namespace Ocluse.LiquidSnow.Cryptography.IO
{
    /// <summary>
    /// Contains utility methods for handling, encrypting and decrypting several items/files in a single package.
    /// </summary>
    public interface ICryptoContainer : IDisposable
    {
        /// <summary>
        /// The key used for encrypting/decrypting the items
        /// </summary>
        byte[] Key { get; }

        /// <summary>
        /// Adds an item to the container
        /// </summary>
        /// <param name="name">The name the item will be stored under</param>
        /// <param name="data">The data to be stored</param>
        /// <param name="overwrite">If true and the item already exists, it will be overwritten. If false and the item already exists, an <see cref="IOException"/> is thrown</param>
        /// <param name="progress">The subscriber notified of the progress of the operation</param>
        /// <param name="cancellationToken">The token used to monitor for cancellation requests</param>
        Task AddStreamAsync(string name, Stream data, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        ///<inheritdoc cref="AddStreamAsync(string, Stream, bool, IProgress{double}, CancellationToken)"/>
        Task AddBytesAsync(string name, byte[] data, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="AddStreamAsync(string, Stream, bool, IProgress{double}, CancellationToken)"/>
        Task AddTextAsync(string name, string data, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an item from the container
        /// </summary>
        /// <param name="name">The name of the item to be deleted</param>
        /// <returns>True if the file was deleted. False if the item was not found in the container</returns>
        Task<bool> DeleteAsync(string name);

        /// <summary>
        /// Determines if an item exists in the container
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns>True if the item exists. False if not found</returns>
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts all the items in the container to the provided directory
        /// </summary>
        /// <param name="outputDirectory">The directory path to extract the items to</param>
        /// <param name="progress">The subscriber notified of the operation progress</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns></returns>
        Task ExtractContainerAsync(string outputDirectory, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the URIs of all the items in the container.
        /// </summary>
        /// <returns></returns>
        Task<List<string>> EnumerateItemsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets and decrypt an item from the container
        /// </summary>
        /// <param name="name">The name of the item to get</param>
        /// <param name="output">The destination <see cref="Stream"/> of the decrypted item</param>
        /// <param name="progress">The subscriber notified of the operation progress</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        Task GetStreamAsync(string name, Stream output, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        ///<inheritdoc cref="GetStreamAsync(string, Stream, IProgress{double}, CancellationToken)"/>
        ///<returns>The data in bytes</returns>
        Task<byte[]> GetBytesAsync(string name, IProgress<double>? progress = null, CancellationToken cancellationToken = default);

        ///<inheritdoc cref="GetTextAsync(string, IProgress{double}, CancellationToken)"/>
        ///<returns>The data as a UTF8 string</returns>
        Task<string> GetTextAsync(string name, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
    }
}