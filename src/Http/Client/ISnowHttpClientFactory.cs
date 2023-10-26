namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// A factory for creating <see cref="HttpClient"/> instances with a name;
    /// </summary>
    public interface ISnowHttpClientFactory
    {
        /// <summary>
        /// Creates a <see cref="HttpClient"/> with the given name.
        /// </summary>
        Task<HttpClient> CreateClient(string? name, CancellationToken cancellationToken = default);
    }
}
