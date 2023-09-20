using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class InputBase<TValue> : ControlBase, IValidatable, IDisposable
    {
        //private Timer? _debounceTimer;
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
        public ValidationResult ValidationResult { get; set; } = ValidationResult.Succeeded();

        [Parameter]
        public EventCallback<ValidationResult> ValidationResultChanged { get; set; }

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
            if (!EnableDebounce && _debounceSubscription!=null)
            {
                _debounceSubscription.Dispose();
            }
        }

        protected async Task OnChange(ChangeEventArgs e)
        {
            Value = GetValue(e.Value);

            await ValueChanged.InvokeAsync(Value);

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
            else
            {
                await InvokeValidate();
            }
        }

        private async void OnUserFinish()
        {
            await InvokeValidate();
            await UserFinished.InvokeAsync();
        }

        protected abstract TValue? GetValue(object? value);

        public virtual async Task<bool> InvokeValidate()
        {
            if (Validate != null)
            {
                var validationResult = await Validate.Invoke(Value);
                ValidationResult = validationResult;
                await ValidationResultChanged.InvokeAsync(validationResult);
                return ValidationResult.Success;
            }
            else
            {
                return true;
            }
        }

        protected override sealed void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);

            BuildInputClass(classList);

            if (!ValidationResult.Success)
            {
                classList.Add("error");
            }

            if (Disabled)
            {
                classList.Add(DisabledClass ?? "disabled");
            }

            if (ReadOnly)
            {
                classList.Add(ReadOnlyClass ?? "read-only");
            }

            if (Value != null)
            {
                classList.Add(HasValueClass ?? "has-value");
            }
        }

        protected virtual void BuildInputClass(List<string> classList)
        {

        }

        protected string GetValidationClass()
        {
            return new ClassBuilder()
                .Add(ValidationResult.Success ? "validation-success" : "validation-error")
                .Build();
        }

        public void Dispose()
        {
            _debounceSubscription?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
