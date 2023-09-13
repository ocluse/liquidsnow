using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// Provides utility methods for making CRUD requests in a RESTful manner
    /// </summary>
    /// <typeparam name="TCreate">The request sent to create a resource</typeparam>
    /// <typeparam name="TUpdate">The request sent to update a resource</typeparam>
    /// <typeparam name="TList">The request sent as a query to fetch a list of items</typeparam>
    /// <typeparam name="TModel">The main model type of the resource received from the server</typeparam>
    /// <typeparam name="TSummary">The model type received from the server for LIST/QUERY operations</typeparam>
    public interface ICrudRequestBuilder<TCreate, TUpdate, TList, TModel, TSummary> where TUpdate : IKeyCommand<TModel>
    {
        /// <summary>
        /// Sends a request to create a resource
        /// </summary>
        Task<TModel> CreateAsync(TCreate create, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a request to read a resource
        /// </summary>
        Task<TModel> ReadAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a request to update a resource
        /// </summary>
        Task<TModel> UpdateAsync(TUpdate update, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a request to delete a resource
        /// </summary>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send a request to query a list of resources
        /// </summary>
        Task<QueryResult<TSummary>> ListAsync(TList? list, CancellationToken cancellationToken = default);
    }

    ///<inheritdoc/>
    public interface ICrudRequestBuilder<TCreate, TUpdate, TList, TModel> : ICrudRequestBuilder<TCreate, TUpdate, TList, TModel, TModel>
        where TUpdate : IKeyCommand<TModel>
    {
    }
}