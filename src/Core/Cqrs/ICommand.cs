using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Describes an instruction that is issued to an application to perform an operation. 
/// </summary>
public interface ICommand<TCommandResult>
{
}

///<inheritdoc cref="ICommand{TCommandResult}"/>
public interface ICommand : ICommand<Unit>
{

}
