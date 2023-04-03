
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Akka.Actor;
using Akka.Dispatch;

namespace Aaron.Infrastructure.Monitoring;

public class TraceableDispatcher : Dispatcher
{
    private static readonly Func<Mailbox, object?> GetMailboxActorField = BuildActorAccessor<Mailbox>("_actor");
    private readonly TraceableDispatcherOptions _options;

    public TraceableDispatcher(MessageDispatcherConfigurator configurator,
        TraceableDispatcherOptions options,
        string id,
        int throughput,
        long? throughputDeadlineTime,
        ExecutorServiceFactory executorServiceFactory,
        TimeSpan shutdownTimeout)
        : base(configurator, id, throughput, throughputDeadlineTime, executorServiceFactory, shutdownTimeout)
    {
        _options = options;
    }

    public override void Dispatch(ActorCell cell, Envelope envelope)
    {
        if (_options.TraceableAssemblies.Contains(cell.Props.Type.Assembly))
        {
            TraceableDispatcherMetrics.MessageDispatched(cell.System.Name, cell.GetActorNameSafe(), envelope.Message.GetType().Name);
        }

        base.Dispatch(cell, envelope);
    }

    protected override void ExecuteTask(IRunnable run)
    {
        if (run is Mailbox mailbox)
        {
            var fieldValue = GetMailboxActorField(mailbox);
            if (fieldValue is ActorCell cell && _options.TraceableAssemblies.Contains(cell.Props.Type.Assembly))
            {
                run = new MailboxWrapper(mailbox, cell.Props.Type);
            }
        }

        base.ExecuteTask(run);
    }

    private class MailboxWrapper : IRunnable
    {
        private readonly Mailbox _mailbox;
        private readonly Type _actorType;

        public MailboxWrapper(Mailbox mailbox, Type actorType)
        {
            _mailbox = mailbox;
            _actorType = actorType;
        }

        public void Run()
        {
            var systemName = _mailbox.Dispatcher.Configurator.Prerequisites.Settings.System.Name;
            TraceableDispatcherMetrics.MailboxSize(systemName, _actorType.Name, _mailbox.MessageQueue.Count);

            var st = Stopwatch.StartNew();
            _mailbox.Run();
            st.Stop();

            TraceableDispatcherMetrics.MailboxProcessed(systemName, _actorType.Name, st.ElapsedMilliseconds);
        }

#if !NETSTANDARD
        public void Execute()
        {
            Run();
        }
#endif
    }

    private static Func<TSource, object?> BuildActorAccessor<TSource>(string fieldName)
    {
        var sourceType = typeof(TSource);
        var field = sourceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null)
        {
            throw new InvalidOperationException($"Unable to initialize '{nameof(TraceableDispatcher)}'. No private field '_actor' found on '{sourceType.Name}'.");
        }

        var source = Expression.Parameter(sourceType, "cell");
        var fieldAccess = Expression.Field(source, field);
        var lambda = Expression.Lambda<Func<TSource, object>>(fieldAccess, source);

        return lambda.Compile();
    }
}
