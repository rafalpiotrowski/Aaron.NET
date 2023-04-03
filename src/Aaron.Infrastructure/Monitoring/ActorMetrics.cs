using System.Diagnostics;
using System.Diagnostics.Metrics;
using Akka.Actor;

namespace Aaron.Infrastructure.Monitoring;

public class ActorMetrics
{
    private static readonly Meter Meter = new(TelemetryConstants.TelemetryNamespace);
    private static readonly Counter<long> MessagesReceived = Meter.CreateCounter<long>("akka_messages_received", "total");
    private static readonly UpDownCounter<long> ActiveActors = Meter.CreateUpDownCounter<long>("akka_active_actors", "total");
    private static readonly UpDownCounter<long> ActorRestarts = Meter.CreateUpDownCounter<long>("akka_actor_restarts", "total");

    public static void MessageReceived(IActorContext context, object message, bool handled)
    {
        MessagesReceived.Add(1, new TagList
        {
            { TelemetryConstants.SystemTagName, context.System.Name },
            { TelemetryConstants.ActorTagName, context.GetActorNameSafe() },
            { TelemetryConstants.MessageTagName, message.GetType().Name },
            { TelemetryConstants.HandledTagName, handled }
        });
    }
    
    public static void AddActiveActors(IActorContext context, long value)
    {
        ActiveActors.Add(value, new TagList
        {
            { TelemetryConstants.SystemTagName, context.System.Name },
            { TelemetryConstants.ActorTagName, context.GetActorNameSafe() }
        });
    }

    public static void ActorRestarted(IActorContext context)
    {
        ActorRestarts.Add(1, new TagList
        {
            { TelemetryConstants.SystemTagName, context.System.Name },
            { TelemetryConstants.ActorTagName, context.GetActorNameSafe() }
        });
    }
}
