
using Akka.Configuration;
using Akka.Dispatch;

namespace Aaron.Infrastructure.Monitoring;

public class TraceableDispatcherConfigurator : MessageDispatcherConfigurator
{
    private readonly Dispatcher _instance;

    public TraceableDispatcherConfigurator(Config config, IDispatcherPrerequisites prerequisites)
        : base(config, prerequisites)
    {
        var deadlineTime = Config.GetTimeSpan("throughput-deadline-time", null);
        long? deadlineTimeTicks = null;
        if (deadlineTime.Ticks > 0)
        {
            deadlineTimeTicks = deadlineTime.Ticks;
        }

        if (Config.IsNullOrEmpty())
        {
            throw ConfigurationException.NullOrEmptyConfig<DispatcherConfigurator>();
        }

        var dispatcherOptions = prerequisites.Settings.Setup.Get<TraceableDispatcherOptions>();
        if (dispatcherOptions.IsEmpty)
        {
            throw ConfigurationException.NullOrEmptyConfig<TraceableDispatcherOptions>();
        }

        _instance = new TraceableDispatcher(this,
            dispatcherOptions.Value,
            Config.GetString("id"),
            Config.GetInt("throughput"),
            deadlineTimeTicks,
            ConfigureExecutor(),
            Config.GetTimeSpan("shutdown-timeout"));
    }

    public override MessageDispatcher Dispatcher()
    {
        return _instance;
    }
}