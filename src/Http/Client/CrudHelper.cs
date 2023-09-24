using Ocluse.LiquidSnow.Http.Cqrs;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// Helper methods for CRUD request builders, to make it easier to perform queries.
    /// </summary>
    public static class CrudHelper
    {
        /// <summary>
        /// The default page size to use when querying.
        /// </summary>
        public static int DefaultPageSize { get; set; } = 20;

        /// <summary>
        /// Sends a request to query for a list of resources using offset paging.
        /// </summary>
        public static async Task<QueryResult<TModel>> QueryAsync<TCreate, TUpdate, TList, TModel>(
            this CrudRequestBuilder<TCreate, TUpdate, TList, TModel> crudBuilder,
            int page,
            string? search = null,
            int? pageSize = null)
            where TUpdate : IKeyCommand<TModel>
            where TList : ListQuery<TModel>
        {
            TList query = Activator.CreateInstance<TList>();
            query.Page = page;
            query.Search = search;
            query.Size = pageSize ?? DefaultPageSize;
            query.QType = QueryType.Offset;

            return await crudBuilder.ListAsync(query);
        }

        /// <summary>
        /// Sends a request to query for a list of resources using cursor paging.
        /// </summary>
        public static async Task<QueryResult<TModel>> QueryAsync<TCreate, TUpdate, TList, TModel>(
            this CrudRequestBuilder<TCreate, TUpdate, TList, TModel> crudBuilder,
            string? cursor,
            int? pageSize = null)
            where TUpdate : IKeyCommand<TModel>
            where TList : ListQuery<TModel>
        {
            TList query = Activator.CreateInstance<TList>();
            query.Cursor = cursor;
            query.Size = pageSize ?? DefaultPageSize;
            query.QType = QueryType.Cursor;

            return await crudBuilder.ListAsync(query);
        }
    }
}
