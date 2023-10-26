using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class InputBase<TValue> : ControlBase, IValidatable, IDisposable
    {
        private bool _valueHasChanged;
        private IDisposable? _debounceSubscription;

        [Parameter]
        public TValue? Value { get; set; }

        [Parameter]
        public EventCallback<TValue?> ValueChanged { get; set; }

        [Parameter]
        public string? Placeholder { get; set; }

        [Parameter]
        public string? Header { get; set; }

        [Parameter]
        public ValidationResult? Validation { get; set; }

        [Parameter]
        public EventCallback<ValidationResult?> ValidationChanged { get; set; }

        [Parameter]
        public Func<TValue?, Task<ValidationResult>>? Validate { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string? DisabledClass { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public string? ReadOnlyClass { get; set; }

        [Parameter]
        public string? HasValueClass { get; set; }

        [Parameter]
        public bool EnableDebounce { get; set; }

        [Parameter]
        public int DebounceInterval { get; set; } = 500;

        [Parameter]
        public EventCallback UserFinished { get; set; }

        protected override void OnParametersSet()
        {
            if (!EnableDebounce && _debounceSubscription != null)
            {
                _debounceSubscription.Dispose();
            }
        }

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

        private async void OnUserFinish()
        {
            await InvokeAsync(InvokeValidate);
            await UserFinished.InvokeAsync();
        }

        protected abstract TValue? GetValue(object? value);

        public virtual async Task<bool> InvokeValidate()
        {
            bool result;
            if (Validate != null)
            {
                var validationResult = await Validate.Invoke(Value);
                await ValidationChanged.InvokeAsync(validationResult);
                result = validationResult.Success;
            }
            else
            {
                result = true;
            }
            await InvokeAsync(StateHasChanged);

            return result;
        }

        protected override sealed void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);

            BuildInputClass(classBuilder);

            if (Validation?.Success == false)
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

        protected virtual void BuildInputClass(ClassBuilder classBuilder)
        {

        }

        protected string GetValidationClass()
        {
            return new ClassBuilder()
                .AddIf(Validation?.Success == true, "validation-success")
                .AddIf(Validation?.Success == false, "validation-error")
                .Build();
        }

        public void Dispose()
        {
            _debounceSubscription?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
