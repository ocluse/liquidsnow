using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// Provides utility methods for making CRUD requests in a REST-ful manner
    /// </summary>
    /// <typeparam name="TCreate">The request sent to create a resource</typeparam>
    /// <typeparam name="TUpdate">The request sent to update a resource</typeparam>
    /// <typeparam name="TList">The request sent as a query to fetch a list of items</typeparam>
    /// <typeparam name="TModel">The main model type of the resource received from the server</typeparam>
    /// <typeparam name="TSummary">The model type received from the server for LIST/QUERY operations</typeparam>
    public interface ICrudRequestBuilder<TCreate, TUpdate, TList, TModel, TSummary> :
        ICreateRequestBuilder<TCreate, TModel>,
        IReadRequestBuilder<TModel>,
        IUpdateRequestBuilder<TUpdate, TModel>,
        IDeleteRequestBuilder,
        IListRequestBuilder<TList, TSummary>
        where TUpdate : IKeyCommand<TModel>
        
    {
        /// <summary>
        /// The REST URl endpoint of the resource.
        /// </summary>
        string Path { get; }
    }

    ///<inheritdoc/>
    public interface ICrudRequestBuilder<TCreate, TUpdate, TList, TModel> : ICrudRequestBuilder<TCreate, TUpdate, TList, TModel, TModel>
        where TUpdate : IKeyCommand<TModel>
    {
    }

    /// <summary>
    /// A utility contract for making a request to create a resource
    /// </summary>
    public interface ICreateRequestBuilder<TCreate, TResult>
    {
        /// <summary>
        /// Sends a request to create a resource
        /// </summary>
        Task<TResult> CreateAsync(TCreate create, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A utility contract for making a request to read a resource
    /// </summary>
    public interface IReadRequestBuilder<TResult>
    {
        /// <summary>
        /// Sends a request to read a resource
        /// </summary>
        Task<TResult> ReadAsync(string id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A utility contract for making a request to update a resource
    /// </summary>
    public interface IUpdateRequestBuilder<TUpdate, TResult> 
        where TUpdate : IKeyCommand<TResult>
    {
        /// <summary>
        /// Sends a request to update a resource
        /// </summary>
        Task<TResult> UpdateAsync(TUpdate update, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A utility contract for making a request to delete a resource
    /// </summary>
    public interface IDeleteRequestBuilder
    {
        /// <summary>
        /// Sends a request to delete a resource
        /// </summary>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A utility contract for making a request to query a list of resources
    /// </summary>
    public interface IListRequestBuilder<TList, TResult>
    {
        /// <summary>
        /// Send a request to query a list of resources
        /// </summary>
        Task<QueryResult<TResult>> ListAsync(TList? list, CancellationToken cancellationToken = default);
    }
}