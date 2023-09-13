using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// A task and its description, that typically instructs the application to create, update or delete resources
    /// </summary>
    /// <typeparam name="TCommandResult">The expected result after execution of the command</typeparam>
    public interface ICommand<TCommandResult>
    {
    }

    ///<inheritdoc cref="ICommand{TCommandResult}"/>
    public interface ICommand : ICommand<Unit>
    {

    }
}
