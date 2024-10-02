using Ocluse.LiquidSnow.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// An event that is raised when an <see cref="IJob"/> fails.
/// </summary>
/// <param name="Job">The job that failed</param>
/// <param name="Exception">The exception that was thrown while executing the job</param>
/// <remarks>
/// This event will only be fired when the executing application has registered the <see cref="IEventBus"/> or a handler(s) for this event.
/// </remarks>
public record JobFailedEvent(IJob Job, Exception Exception) : IEvent;
