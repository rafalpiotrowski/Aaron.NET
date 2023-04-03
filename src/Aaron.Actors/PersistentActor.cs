using Aaron.Infrastructure.Monitoring;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;
using Akka.Persistence;

namespace Aaron.Actors;

/// <summary>
/// Base class for persistent actor.
/// Handles global metrics for actor
/// </summary>
public abstract class PersistentActor : Akka.Persistence.ReceivePersistentActor 
#if PHOBOS
    , Phobos.Actor.Common.IInstrumented
#endif
{
    protected readonly ILoggingAdapter Logger;
    protected string SenderPath => Sender.IsNobody() ? "Nobody" : Sender.Path.ToString();

    /// <summary>
    /// OnSaveSnapshotSuccess will call this when deleting snapshots
    /// by default every 100 snapshots we will try to remove snapshots older then 30 days but the last snapshot
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    protected virtual SnapshotSelectionCriteria SnapshotSelectionCriteria(SaveSnapshotSuccess msg) => 
        new(msg.Metadata.SequenceNr - 1, DateTime.UtcNow.AddDays(-30));

    /// <summary>
    /// OnSaveSnapshotSuccess will call this method to check if we should remove old snapshots
    /// by default every 100 snapshots we will try to remove snapshots older then 30 days but the last snapshot
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    protected virtual bool CheckIfCleanSnapshotsRequired(SaveSnapshotSuccess msg) => 
        msg.Metadata.SequenceNr % 100 == 0;

    protected PersistentActor()
    {
        Logger = Context.GetLogger();

        Recover<RecoveryCompleted>(e =>
        {
            Logger.Debug($"RecoveryCompleted");
        });
    }

    /// <summary>
    /// Correct way to stop persistent actor using passivate message with poison pill
    /// </summary>
    protected void StopSelf() => Context.Parent.Tell(new Passivate(PoisonPill.Instance));

    /// <summary>
    /// If the actor receives a message for which no handler matches, the unhandled message is published to the EventStream wrapped in an UnhandledMessage
    /// </summary>
    /// <param name="message"></param>
    protected override void Unhandled(object message)
    {
        //AkkaMonitoringExtension, is also logging akka.messages.unhandled
        //but without actor context. With this counter we keep the context
        //so we can be more specific when designing dashboard
        Logger.Warning($"Unhandled message type: {message.GetType().Name}");
        base.Unhandled(message);
    }

    public override void AroundPreStart()
    {
        Logger.Debug("AroundPreStart");
        base.AroundPreStart();
    }

    /// <summary>
    /// called on new instance of an actor
    /// after new instance is created
    /// </summary>
    protected override void PreStart()
    {
        ActorMetrics.AddActiveActors(Context, 1);
        Logger.Debug("PreStart");
        base.PreStart();
    }

    public override void AroundPostStop()
    {
        Logger.Debug("AroundPostStop");
        // if(this is IWithUnboundedStash || this is IWithTimers)
        // {
        //     try
        //     {
        //         base.AroundPostStop();
        //     }
        //     catch (System.NullReferenceException)
        //     {
        //         //Logger.Warning($"base.AroundPostStop for {this.GetType().Name} : IWithUnboundedStash or IWithTimers failed");
        //         //seems to be a bug in akka.net when persistent actor is IWithTimers or IWithUnboundedStash
        //     }
        // }
        // else
            base.AroundPostStop();
    }

    /// <summary>
    /// After stopping an actor
    /// called on instance when:
    /// Stop or Context.Stop() or PoisonPill is received
    /// Terminated is send to Watchers
    /// path will be allowed to be used again
    /// </summary>
    protected override void PostStop()
    {
        ActorMetrics.AddActiveActors(Context, -1);

        // try
        // {
            Logger.Debug("Stopped");
            base.PostStop();
        // }
        // catch (NullReferenceException ex)
        // {
        //     Logger.Error("Error in PostStop", ex);
        // }
    }

    /// <summary>
    /// called on old instance of an Actor
    /// before new instance is created
    /// </summary>
    /// <param name="cause"></param>
    /// <param name="message"></param>
    public override void AroundPreRestart(Exception cause, object message)
    {
        Logger.Debug("AroundPreRestart");
        base.AroundPreRestart(cause, message);
    }

    public override void AroundPostRestart(Exception cause, object message)
    {
        Logger.Debug("AroundPostRestart");
        base.AroundPostRestart(cause, message);
    }

    /// <summary>
    /// called on new instance of an Actor
    /// </summary>
    /// <param name="reason"></param>
    protected override void PostRestart(Exception reason)
    {
        ActorMetrics.ActorRestarted(Context);
        Logger.Debug("PostRestart");
        base.PostRestart(reason);
    }

    protected override bool AroundReceive(Receive receive, object message)
    {
        if (Logger.IsDebugEnabled) Logger.Debug($"ArroundReceive: {message.GetType().Name}");
        var handled = base.AroundReceive(receive, message);
        ActorMetrics.MessageReceived(Context, message, handled);
        return handled;
    }

    protected override void PreRestart(Exception reason, object message)
    {
        Logger.Debug("PreRestart");
        base.PreRestart(reason, message);
    }

    protected override void OnReplaySuccess()
    {
        Logger.Debug($"OnReplaySuccess");
        base.OnReplaySuccess();
    }

    protected void HandleSnapshotMessages()
    {
        Command<SaveSnapshotSuccess>(msg =>
        {
            Logger.Info($"SaveSnapshotSuccess PersistanceId: {msg.Metadata.PersistenceId} SequenceId: {msg.Metadata.SequenceNr} Timestamp: {msg.Metadata.Timestamp}");
            OnSaveSnapshotSuccess(msg);
        });
        Command<SaveSnapshotFailure>(msg =>
        {
            Logger.Error(msg.Cause, $"SaveSnapshotFailure PersistanceId: {msg.Metadata.PersistenceId} SequenceId: {msg.Metadata.SequenceNr} Timestamp: {msg.Metadata.Timestamp}");
        });
        Command<DeleteSnapshotSuccess>(msg =>
        {
            Logger.Info($"DeleteSnapshotSuccess PersistanceId: {msg.Metadata.PersistenceId} SequenceId: {msg.Metadata.SequenceNr} Timestamp: {msg.Metadata.Timestamp}");
        });
        Command<DeleteSnapshotFailure>(msg =>
        {
            Logger.Error(msg.Cause, $"DeleteSnapshotFailure PersistanceId: {msg.Metadata.PersistenceId} SequenceId: {msg.Metadata.SequenceNr} Timestamp: {msg.Metadata.Timestamp}");
        });
        Command<DeleteSnapshotsSuccess>(msg =>
        {
            Logger.Info($"DeleteSnapshotsSuccess MinSequenceNr: {msg.Criteria.MinSequenceNr} MaxSequenceNr: {msg.Criteria.MaxSequenceNr} MinTimestamp: {msg.Criteria.MinTimestamp} MaxTimeStamp: {msg.Criteria.MaxTimeStamp}");
        });
        Command<DeleteSnapshotsFailure>(msg =>
        {
            Logger.Error($"DeleteSnapshotsFailure MinSequenceNr: {msg.Criteria.MinSequenceNr} MaxSequenceNr: {msg.Criteria.MaxSequenceNr} MinTimestamp: {msg.Criteria.MinTimestamp} MaxTimeStamp: {msg.Criteria.MaxTimeStamp}");
        });
    }

    /// <summary>
    /// by default every 100 snapshots we will try to remove snapshots older then 30 days but the last snapshot
    /// </summary>
    /// <param name="msg"></param>
    protected virtual void OnSaveSnapshotSuccess(SaveSnapshotSuccess msg)
    {
        if (CheckIfCleanSnapshotsRequired(msg))
            DeleteSnapshots(SnapshotSelectionCriteria(msg));
    }
}
