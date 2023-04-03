using Aaron.Infrastructure.Monitoring;
using Akka.Actor;
using Akka.Event;

namespace Aaron.Actors;

public abstract class Actor: ReceiveActor
#if PHOBOS
    , Phobos.Actor.Common.IInstrumented
#endif
{
    protected readonly ILoggingAdapter Logger = Context.GetLogger();

    /// <summary>
    /// This should not be called if the actor is Sharded/Persistent
    /// </summary>
    protected void StopSelf() => Context.Stop(Self);

    /// <summary>
    /// If the actor receives a message for which no handler matches, the unhandled message is published to the EventStream wrapped in an UnhandledMessage
    /// </summary>
    /// <param name="message"></param>
    protected override void Unhandled(object message)
    {
        //AkkaMonitoringExtension, is also logging akka.messages.unhandled
        //but without actor context. With this counter we keep the context
        //so we can be more specific when designing dashboard
        Logger.Warning($"Unhandled: {message.GetType().Name}");
        base.Unhandled(message);
    }

    /// <summary>
    /// When an actor is scheduled for a restart, its PreRestart method is called. This is a lifecycle hook
    /// that allows you to perform tasks like resource cleanup, stopping child actors,
    /// or any other necessary preparations before the actor is restarted. By default,
    /// the PreRestart method stops all the actor's children,
    /// but you can override this method to customize its behavior if needed.
    /// </summary>
    protected override void PreStart()
    {
        ActorMetrics.AddActiveActors(Context, 1);
        base.PreStart();
    }

    /// <summary>
    /// After stopping an actor
    /// called on instance when:
    /// Stop or Context.Stop() or PoisonPill is received
    /// Terminated is send to Watchers
    /// path will be allowed to be used again
    ///
    /// After the PreRestart method has been executed, the PostStop method is called.
    /// This is another lifecycle hook that can be used to perform additional cleanup tasks,
    /// release resources, or execute any other actions required after the actor has stopped.
    /// This method can also be overridden to provide custom functionality.
    /// </summary>
    protected override void PostStop()
    {
        ActorMetrics.AddActiveActors(Context, -1);

        // try
        // {
            base.PostStop();
        // }
        // catch (NullReferenceException ex)
        // {
        //     Logger.Error("Error in PostStop", ex);
        //     //throw;
        // }
    }

    /// <summary>
    /// called on new instance of an Actor
    ///
    /// Once the actor has been re-created, its PostRestart method is called.
    /// This lifecycle hook is intended for performing initialization tasks after the actor has been restarted.
    /// By default, the PostRestart method calls the PreStart method,
    /// which allows you to reuse the same initialization logic that was applied when the actor was first created.
    /// However, you can override the PostRestart method to implement custom initialization
    /// or other actions specifically tailored to the restart scenario.
    /// Message processing (resumed):
    /// After the PostRestart method has been executed, the actor resumes its message processing loop.
    /// It starts handling incoming messages again, using its new instance.
    /// Note that the messages that were in the actor's mailbox at the time of the restart are not lost;
    /// they will be processed by the restarted actor.
    ///
    /// It's important to note that the restart strategy is determined by the actor's supervisor.
    /// In Akka.NET, each actor has a supervisor, which can be either its parent actor or a designated supervisor
    /// specified in the actor's configuration. The supervisor is responsible for handling the failures of
    /// its child actors and deciding what action to take, such as restarting, stopping,
    /// or escalating the failure to its own supervisor.
    /// This hierarchical supervision model allows for better fault-tolerance and error recovery in distributed systems.
    /// </summary>
    /// <param name="reason"></param>
    protected override void PostRestart(Exception reason)
    {
        ActorMetrics.ActorRestarted(Context);
        base.PostRestart(reason);
    }

    /// <summary>
    /// AroundReceive is a method in Akka.NET that can be used to intercept incoming messages
    /// and apply custom behavior before or after the message is processed by the actor.
    /// By default, AroundReceive simply calls the OnReceive (for untyped actors)
    /// or Receive (for typed actors) method to process the message.
    /// However, you can override AroundReceive to add custom behavior,
    /// such as logging, message filtering, or other cross-cutting concerns.
    ///
    /// When you override the AroundReceive method, you need to ensure that the message processing logic
    /// is still executed, either before or after your custom code.
    /// To do this, you should call the base implementation of AroundReceive within your overridden method. 
    /// </summary>
    /// <param name="receive"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected override bool AroundReceive(Receive receive, object message)
    {
        var handled = base.AroundReceive(receive, message);
        ActorMetrics.MessageReceived(Context, message, handled);
        return handled;
    }
}
