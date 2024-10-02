using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for controls that accept user input.
/// </summary>
/// <typeparam name="TValue">The type of the value accepted by the input</typeparam>
public abstract class InputBase<TValue> : ControlBase, IValidatable, IInput, IDisposable
{
    private bool _valueHasChanged;
    private bool _disposedValue;
    private IDisposable? _debounceSubscription;
    private readonly string _defaultName = Guid.NewGuid().ToString();

    /// <summary>
    /// The content to display before the input.
    /// </summary>
    [Parameter]
    public RenderFragment? PrefixContent { get; set; }

    /// <summary>
    /// The content to display after the input.
    /// </summary>
    [Parameter]
    public RenderFragment? SuffixContent { get; set; }

    /// <summary>
    /// The content to display in the validation area of the input.
    /// </summary>
    [Parameter]
    public RenderFragment? ValidationContent { get; set; }

    /// <summary>
    /// The currently selected value of the input.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// A callback for when the value of the input changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the placeholder to display when the input is empty.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the header of the input.
    /// </summary>
    [Parameter]
    public string? Header { get; set; }

    /// <summary>
    /// Gets or sets the 'name' of the input.
    /// </summary>
    /// <remarks>
    /// By default, the 'name' is generated from a random Guid if not provided.
    /// If a <see cref="Header"/> is provided, it will be used as the actual name.
    /// </remarks>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the validation result of the input.
    /// </summary>
    [Parameter]
    public ValidationResult? Validation { get; set; }

    /// <summary>
    /// Gets or sets the callback for when the validation result changes.
    /// </summary>
    [Parameter]
    public EventCallback<ValidationResult?> ValidationChanged { get; set; }

    /// <summary>
    /// Gets or sets the function to validate the input value.
    /// </summary>
    [Parameter]
    public Func<TValue?, Task<ValidationResult>>? Validate { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input is disabled.
    /// </summary>
    /// <remarks>
    /// If the class is not provided, a default 'disabled' class will be added instead.
    /// </remarks>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets a value that, if true, a readonly attribute will be added to the input.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input is readonly.
    /// </summary>
    /// <remarks>
    /// If not provided, a default 'read-only' class will be added instead.
    /// </remarks>
    [Parameter]
    public string? ReadOnlyClass { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input has a value that is not null.
    /// </summary>
    [Parameter]
    public string? HasValueClass { get; set; }

    /// <summary>
    /// Gets or sets a value that, if true, will enable debounce for the input.
    /// </summary>
    [Parameter]
    public bool EnableDebounce { get; set; }

    /// <summary>
    /// Gets or sets the debouncing interval that after which, the <see cref="UserFinished"/> callback is invoked.
    /// </summary>
    [Parameter]
    public int DebounceInterval { get; set; } = 500;

    /// <summary>
    /// Gets or sets a callback invoked once the user has finished interacting with the input when <see cref="EnableDebounce"/> is true.
    /// </summary>
    [Parameter]
    public EventCallback UserFinished { get; set; }

    /// <summary>
    /// The containing from parent.
    /// </summary>
    [CascadingParameter]
    public IForm? FormContainer { get; private set; }

    /// <summary>
    /// The final name that is added as an attribute.
    /// </summary>
    protected string AppliedName => string.IsNullOrEmpty(Name) ? string.IsNullOrEmpty(Header) ? _defaultName : Header : Name;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormContainer?.Register(this);
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (!EnableDebounce && _debounceSubscription != null)
        {
            _debounceSubscription.Dispose();
        }
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        var newValue = parameters.GetValueOrDefault<TValue>(nameof(Value));

        bool valueChanged = !EqualityComparer<TValue>.Default.Equals(Value, newValue);

        await base.SetParametersAsync(parameters);

        // Since debounce will validate on user finish, we need to validate here if debounce is disabled
        // We also only want to validate if the value has changed.
        if (valueChanged && !EnableDebounce && _valueHasChanged)
        {
            await InvokeAsync(InvokeValidate);
        }
    }

    /// <summary>
    /// Invoked when the value of the input changes.
    /// </summary>
    protected async Task OnChange(ChangeEventArgs e)
    {
        var newValue = GetValue(e.Value);

        await ValueChanged.InvokeAsync(newValue);
        _valueHasChanged = true;
        _debounceSubscription?.Dispose();

        if (EnableDebounce)
        {
            _debounceSubscription = Observable
                .Timer(TimeSpan.FromMilliseconds(DebounceInterval))
                .Subscribe((t) =>
                {
                    InvokeAsync(OnUserFinish);
                    _debounceSubscription?.Dispose();
                });
        }
    }

    /// <summary>
    /// Invoked once the user has finished interacting with the input.
    /// </summary>
    private async void OnUserFinish()
    {
        await InvokeAsync(InvokeValidate);
        await UserFinished.InvokeAsync();
    }

    /// <summary>
    /// Implemented by inheriting classes to convert the provided value to the input type.
    /// </summary>
    protected abstract TValue? GetValue(object? value);

    /// <summary>
    /// Called to validate the input value.
    /// </summary>
    public virtual async Task<bool> InvokeValidate()
    {
        bool result;
        if (Validate != null)
        {
            var validationResult = await Validate.Invoke(Value);
            await ValidationChanged.InvokeAsync(validationResult);
            result = validationResult.IsValid;
        }
        else
        {
            result = true;
        }
        await InvokeAsync(StateHasChanged);

        return result;
    }

    /// <inheritdoc/>
    protected override sealed void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);

        BuildInputClass(classBuilder);

        if (Validation?.IsValid == false)
        {
            classBuilder.Add("error");
        }

        if (Disabled)
        {
            classBuilder.Add(DisabledClass ?? "disabled");
        }

        if (ReadOnly)
        {
            classBuilder.Add(ReadOnlyClass ?? "read-only");
        }

        if (Value != null)
        {
            classBuilder.Add(HasValueClass ?? "has-value");
        }
    }

    /// <summary>
    /// Allows inheriting classes to add relevant CSS classes to the builder.
    /// </summary>
    /// <param name="classBuilder"></param>
    protected virtual void BuildInputClass(ClassBuilder classBuilder)
    {

    }

    /// <summary>
    /// Gets the CSS class to apply to the validation label depending on the validation state.
    /// </summary>
    protected virtual string GetValidationClass()
    {
        return new ClassBuilder()
            .AddIf(Validation?.IsValid == true, "validation-success")
            .AddIf(Validation?.IsValid == false, "validation-error")
            .Build();
    }

    /// <inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue && disposing)
        {
            FormContainer?.Unregister(this);
            _debounceSubscription?.Dispose();

            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
