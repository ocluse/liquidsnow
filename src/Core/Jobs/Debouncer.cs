using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// A class that provides debouncing functionality.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="Debouncer"/>.
/// </remarks>
/// <param name="interval">The interval in milliseconds after which the <paramref name="execute"/> action should be invoked</param>
/// <param name="execute">The action to execute once the interval has elapsed</param>
public sealed class Debouncer(double interval, Action execute) : IDisposable
{
    private IDisposable? _subscription;
    private bool _disposedValue;
    private bool _running;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Debouncer"/> is running.
    /// </summary>
    public bool Running => _running;

    /// <summary>
    /// Begins the debounce timer. Calling this method will cancel any previously pending timers.
    /// </summary>
    public void Begin()
    {
        Cancel();

        _subscription = Observable
            .Timer(TimeSpan.FromMilliseconds(interval))
            .Subscribe((t) =>
            {
                _running = false;
                execute.Invoke();
            });

        _running = true;
    }

    /// <summary>
    /// Cancels the debounce timer, preventing the <see cref="Action"/> from being invoked.
    /// </summary>
    public void Cancel()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
        _running = false;
        _subscription?.Dispose();
    }

    /// <summary>
    /// Disposes the <see cref="Debouncer"/> and cancels any pending debounce timer.
    /// </summary>
    public void Dispose()
    {
        Cancel();
        _disposedValue = true;
    }
}
