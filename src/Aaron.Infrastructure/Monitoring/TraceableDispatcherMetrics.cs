
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Aaron.Infrastructure.Monitoring;

public static class TraceableDispatcherMetrics
{
    private static readonly Meter Meter = new(TelemetryConstants.TelemetryNamespace);
    private static readonly Counter<long> MessagesDispatchedTotal = Meter.CreateCounter<long>("akka_messages_dispatched", "total");
    private static readonly Counter<long> MailboxSizeCounter = Meter.CreateCounter<long>("akka_mailbox_size", "total");
    private static readonly Histogram<long> MailboxProcessDuration = Meter.CreateHistogram<long>("akka_mailbox_process_duration", "ms");

    public static void MessageDispatched(string systemName, string actorName, string messageType)
    {
        MessagesDispatchedTotal.Add(1, new TagList
        {
            { TelemetryConstants.SystemTagName, systemName },
            { TelemetryConstants.ActorTagName, actorName },
            { TelemetryConstants.MessageTagName, messageType }
        });
    }

    public static void MailboxProcessed(string systemName, string actorName, long duration)
    {
        MailboxProcessDuration.Record(duration, new TagList
        {
            { TelemetryConstants.SystemTagName, systemName },
            { TelemetryConstants.ActorTagName, actorName }
        });
    }

    public static void MailboxSize(string systemName, string actorName, int queueSize)
    {
        MailboxSizeCounter.Add(queueSize, new TagList
        {
            { TelemetryConstants.SystemTagName, systemName },
            { TelemetryConstants.ActorTagName, actorName }
        });
    }
}
