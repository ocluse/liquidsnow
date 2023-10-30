namespace Ocluse.LiquidSnow.Venus.Blazor.Contracts
{
    public interface IFormContainer
    {
        void Register(IInput input);

        void Unregister(IInput input);
    }
}
