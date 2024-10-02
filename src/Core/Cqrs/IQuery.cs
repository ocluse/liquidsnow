namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Describes a task that queries the application for information.
/// </summary>
/// <typeparam name="TResult">The expected result after completion of the query</typeparam>
public interface IQuery<TResult>
{
}
