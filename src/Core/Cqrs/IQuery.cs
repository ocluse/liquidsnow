namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// A fetch operation and it's description, that should not modify the application's 
    /// resources but insteads reads and fetches them.
    /// </summary>
    /// <typeparam name="TResult">The expected result after completion of the query</typeparam>
    public interface IQuery<TResult>
    {
    }
}
