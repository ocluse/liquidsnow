namespace Ocluse.LiquidSnow.Venus.Blazor.Contracts
{
    public interface IDropdownLayout
    {
        event Action<IDropdown>? DropdownClicked;
        event Action Clicked;
        void OnDropdownClicked(IDropdown dropdown);
    }
}
