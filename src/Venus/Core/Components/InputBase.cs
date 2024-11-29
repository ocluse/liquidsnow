using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for controls that accept user input.
/// </summary>
/// <typeparam name="TValue">The type of the value accepted by the input</typeparam>
public abstract class InputBase<TValue> : ControlBase, IValidatable, IFormControl, IDisposable
{
    private bool _valueHasChanged;
    private bool _disposedValue;
    private IDisposable? _debounceSubscription;
    private readonly string _defaultName = IdGenerator.GenerateId(IdKind.Standard, 8);

    /// <summary>
    /// Gets or sets the content to display before the input.
    /// </summary>
    [Parameter]
    public RenderFragment? PrefixContent { get; set; }

    /// <summary>
    /// Gets or sets the content to display after the input.
    /// </summary>
    [Parameter]
    public RenderFragment? SuffixContent { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the validation area of the input.
    /// </summary>
    [Parameter]
    public RenderFragment<ValidationResult?>? ValidationContent { get; set; }

    /// <summary>
    /// Gets or sets the currently selected value of the input.
    /// </summary>
    [Parameter]
    public TValue? Value { get; set; }

    /// <summary>
    /// Gets or sets the callback for when the value changes.
    /// </summary>
    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the header of the input.
    /// </summary>
    [Parameter]
    public string? Header { get; set; }

    /// <summary>
    /// Gets or sets the 'name' attribute of the input.
    /// </summary>
    /// <remarks>
    /// If not provided the <see cref="Header"/> value will be used as the name, 
    /// otherwise it falls back to a randomly generated string.
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
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets a value that determines if the readonly attribute will be added to the input..
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input is readonly.
    /// </summary>
    [Parameter]
    public string? ReadOnlyClass { get; set; }

    /// <summary>
    /// Gets or sets the class to apply when the input has a value that is not null.
    /// </summary>
    [Parameter]
    public string? HasValueClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply when the value of the component is not valid.
    /// </summary>
    [Parameter]
    public string? ErrorClass { get; set; }

    /// <summary>
    /// Gets or sets a value that determines whether to run the debounce logic on value change.
    /// </summary>
    [Parameter]
    public bool EnableDebounce { get; set; }

    /// <summary>
    /// Gets or sets the debouncing interval in milliseconds that after which, the <see cref="UserFinished"/> callback is invoked.
    /// </summary>
    [Parameter]
    public int? DebounceInterval { get; set; }

    /// <summary>
    /// Gets or sets a callback invoked once the user has finished interacting with the input when debouncing is enabled.
    /// </summary>
    [Parameter]
    public EventCallback UserFinished { get; set; }

    [CascadingParameter]
    private IForm? Form { get; set; }

    /// <summary>
    /// Gets the final name that is added as an attribute.
    /// </summary>
    protected string AppliedName => string.IsNullOrEmpty(Name) ? string.IsNullOrEmpty(Header) ? _defaultName : Header : Name;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Form?.Register(this);
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
            _valueHasChanged = false;
            await InvokeAsync(InvokeValidate);
        }
    }

    private async void OnUserFinish()
    {
        await InvokeAsync(InvokeValidate);
        await UserFinished.InvokeAsync();
    }

    /// <summary>
    /// Updates the value of the input and runs validation or debounce.
    /// </summary>
    protected async Task NotifyValueChange(TValue? newValue)
    {
        await ValueChanged.InvokeAsync(newValue);
        _valueHasChanged = true;
        _debounceSubscription?.Dispose();

        if (EnableDebounce)
        {
            _debounceSubscription = Observable
                .Timer(TimeSpan.FromMilliseconds(DebounceInterval ?? Resolver.DefaultDebounceInterval))
                .Subscribe((t) =>
                {
                    InvokeAsync(OnUserFinish);
                    _debounceSubscription?.Dispose();
                });
        }
    }

    /// <inheritdoc/>
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
    protected override sealed void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);

        BuildInputClass(builder);

        bool hasValue = Value is string stringVal ? stringVal.IsNotEmpty() : Value != null;

        builder.AddIf(Validation?.IsValid == false, ClassNameProvider.InputError, ErrorClass)
            .AddIf(Disabled, ClassNameProvider.InputDisabled, DisabledClass)
            .AddIf(ReadOnly, ClassNameProvider.InputReadOnly, ReadOnlyClass)
            .AddIf(hasValue, ClassNameProvider.InputHasValue, HasValueClass);
    }

    /// <summary>
    /// Allows inheriting classes to add relevant CSS classes to the builder.
    /// </summary>
    protected virtual void BuildInputClass(ClassBuilder classBuilder) { }

    /// <summary>
    /// Returns the CSS class to apply to the validation label depending on the validation state.
    /// </summary>
    protected virtual string GetValidationClass()
    {
        return new ClassBuilder()
            .AddIf(Validation?.IsValid == true, ClassNameProvider.ValidationSuccess)
            .AddIf(Validation?.IsValid == false, ClassNameProvider.ValidationError)
            .Build();
    }

    /// <inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue && disposing)
        {
            Form?.Unregister(this);
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
