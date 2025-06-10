using Ocluse.LiquidSnow.Validations;
using Ocluse.LiquidSnow.Venus.Kit.Models;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public partial class Form
{
    private readonly HashSet<IFormControl> _inputs = [];
    private readonly HashSet<IValidatable> _valItems = [];

    [Parameter]
    [EditorRequired]
    public required RenderFragment<FormContext> ChildContent { get; set; }

    [Parameter]
    public string? Feature { get; set; }

    [Parameter]
    public Func<Task>? OnStart { get; set; }

    [Parameter]
    public EventCallback Submitted { get; set; }

    [Parameter]
    public Func<Exception, bool>? SubmissionError { get; set; }

    [CascadingParameter]
    public IPageContext? PageContext { get; set; }


    public bool Enabled { get; set; } = true;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await StartAsync();
    }

    public async Task StartAsync()
    {
        Enabled = false;
        if (OnStart != null)
        {
            await OnStart.Invoke();
        }
        Enabled = true;
    }

    public FormContext GetContext()
    {
        return new FormContext(Enabled, ExecuteSubmit);
    }

    public void Register(IFormControl input)
    {
        _inputs.Add(input);

        if (input is IValidatable val)
        {
            _valItems.Add(val);
        }
    }

    public void Unregister(IFormControl input)
    {
        _inputs.Remove(input);

        if (input is IValidatable val)
        {
            _valItems.Remove(val);
        }
    }

    private async Task ExecuteSubmit()
    {
        if (PageContext is null)
        {
            await ExecuteSubmitCore();
        }
        else
        {
            await PageContext.Execute(ExecuteSubmitCore);
        }
    }

    private async Task ExecuteSubmitCore()
    {
        Dictionary<IFormControl, bool> originalState = [];
        foreach (var input in _inputs)
        {
            originalState.Add(input, input.Disabled);
            input.Disabled = true;
        }

        Enabled = false;

        try
        {
            ValidationSet set = new();
            bool valid = await set.AddRange(_valItems).Validate();

            if (valid)
            {
                try
                {
                    await Submitted.InvokeAsync();
                }
                catch (Exception ex)
                {
                    bool handled = SubmissionError?.Invoke(ex) ?? false;

                    if (!handled)
                    {
                        throw;
                    }
                }
            }
            else
            {
                PageContext?.Snackbar.AddError("Please fix the errors before you can continue");
            }
        }
        finally
        {
            foreach (var input in _inputs)
            {
                input.Disabled = originalState[input];
            }
            Enabled = true;
        }
    }
}