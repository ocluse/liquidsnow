namespace Ocluse.LiquidSnow.Venus.Components;
/// <summary>
/// A component for selecting a single item from a list of items.
/// </summary>
public partial class Dropdown<T>
{
    /// <summary>
    /// The list of options to choose from.
    /// </summary>
    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// A template for rendering each item in the dropdown.
    /// </summary>
    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    /// <summary>
    /// The path to the property that will be used as the display member.
    /// </summary>
    /// <remarks>
    /// <see cref="DisplayMemberFunc"/> is preferred over this property.
    /// </remarks>
    [Parameter]
    public string? DisplayMemberPath { get; set; }

    /// <summary>
    /// A function that returns the display member for the given item.
    /// </summary>
    [Parameter]
    public Func<T?, string>? DisplayMemberFunc { get; set; }

    /// <summary>
    /// The parent layout of the dropdown.
    /// </summary>
    [CascadingParameter]
    public IDropdownLayout? Layout { get; private set; }

    /// <summary>
    /// The CSS class applied to the dropdown when it is open.
    /// </summary>
    [Parameter]
    public string? OpenClass { get; set; }

    /// <summary>
    /// The CSS class applied to the dropdown when it is closed.
    /// </summary>
    [Parameter]
    public string? ClosedClass { get; set; }

    /// <summary>
    /// The CSS class to apply to each item in the dropdown.
    /// </summary>
    [Parameter]
    public string? ItemClass { get; set; }

    /// <summary>
    /// The CSS class to apply to the dropdown list.
    /// </summary>
    [Parameter]
    public string? ListClass { get; set; }

    private bool _isOpen;

    ///<inheritdoc/>
    protected override void OnInitialized()
    {
        if (Layout != null)
        {
            Layout.Clicked += OnLayoutClicked;
            Layout.DropdownClicked += OnDropdownClicked;
        }
    }

    private void OnLayoutClicked()
    {
        SetState(false);
    }

    private void OnDropdownClicked(IDropdown dropdown)
    {
        if (dropdown != this)
        {
            SetState(false);
        }
    }

    private async Task ItemClicked(T item)
    {
        ChangeEventArgs args = new() { Value = item };
        await OnChange(args);
    }

    private void Clicked()
    {
        if (!Disabled)
        {
            SetState(!_isOpen);
            Layout?.OnDropdownClicked(this);
        }
    }

    /// <summary>
    /// Sets the open/close state of the dropdown.
    /// </summary>
    /// <param name="state"></param>
    protected void SetState(bool state)
    {
        InvokeAsync(() =>
       {
           _isOpen = state;
           StateHasChanged();
       });
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);

        classBuilder.Add("dropdown");

        if (_isOpen)
        {
            classBuilder.Add(OpenClass ?? "open");
        }
        else
        {
            classBuilder.Add(ClosedClass ?? "closed");
        }
    }

    ///<inheritdoc/>
    protected override T? GetValue(object? value)
    {
        return (T?)value;
    }

    ///<inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (Layout != null)
        {
            Layout.Clicked -= OnLayoutClicked;
            Layout.DropdownClicked -= OnDropdownClicked;
        }
        base.Dispose(disposing);
    }
}