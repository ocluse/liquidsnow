using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// An event that is raised when an <see cref="IJob"/> fails.
/// </summary>
/// <param name="Job">The job that failed.</param>
/// <param name="Tick">The tick at which the job failed.</param>
/// <param name="Exception">The exception that was thrown while executing the job</param>
/// <remarks>
/// This event will only be fired when the executing application has registered the <see cref="IEventBus"/> or a handler(s) for this event.
/// </remarks>
public record JobFailedEvent(IJob Job, long Tick, Exception Exception);
