using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Channels;

namespace Ocluse.LiquidSnow.Jobs.Internal
{

    internal class JobScheduler(IServiceProvider serviceProvider) : IJobScheduler
    {
        private readonly Dictionary<object, ScheduleHandler> _scheduleHandlers = [];

        private readonly ScheduleHandler _defaultHandler = new(serviceProvider);

        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public event EventHandler<JobFailedEventArgs>? JobFailed;

        private ScheduleHandler GetChannelHandler(IJob job)
        {
            ScheduleHandler? scheduler;

            if(job is IChannelJob channelJob)
            {
                if (!_scheduleHandlers.TryGetValue(channelJob.ChannelId, out scheduler))
                {
                    scheduler = new ScheduleHandler(_serviceProvider);
                    _scheduleHandlers.Add(channelJob.ChannelId, scheduler);
                }
            }
            else
            {
                scheduler = _defaultHandler;
            }
            
            return scheduler;
        }

        public IDisposable Schedule(IJob job)
        {
            var handler = GetChannelHandler(job);
            return handler.Schedule(job);
        }

        public IDisposable Queue(IJob job)
        {
            var handler = GetChannelHandler(job);
            return handler.Queue(job);
        }

        public bool Cancel(object id)
        {
            return _defaultHandler.Cancel(id);
        }

        public bool Cancel(object channelId, object id)
        {
            if (_scheduleHandlers.TryGetValue(channelId, out var handler))
            {
                return handler.Cancel(id);
            }

            return false;
        }
    }
}
