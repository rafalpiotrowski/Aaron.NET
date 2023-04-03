
using Akka.Actor;
using Akka.Hosting;

namespace Aaron.Infrastructure.Monitoring;

public static class AkkaTelemetryExtensions
{
    /// <summary>
    /// Safly get an actor name or 'undefined' <see cref="TelemetryConstants.UndefinedActorName"/> if not possible
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetActorNameSafe(this IActorContext? context)
    {
        return context?.Props?.Type?.Name ?? TelemetryConstants.UndefinedActorName;
    }
    
    public static AkkaConfigurationBuilder WithTraceableDispatcher(this AkkaConfigurationBuilder builder, Action<TraceableDispatcherOptions>? configure = null)
    {
        var options = new TraceableDispatcherOptions();
        configure?.Invoke(options);

        var dispatcherType = typeof(TraceableDispatcherConfigurator).AssemblyQualifiedName;
        return builder
            .AddSetup(options)
            .AddHocon(@$"
                akka.actor.default-dispatcher.type = ""{dispatcherType}""
                akka.remote.use-dispatcher.type = ""{dispatcherType}""",
                HoconAddMode.Prepend);
    }
}