using Aaron.Contracts.Commands;
using Aaron.Contracts.Events;
using Akka.Actor;
#pragma warning disable CS8618

namespace MediaDriver.Actors;

public sealed class DriverActor : Aaron.Actors.Actor, IWithUnboundedStash
{
    public IStash Stash { get; set; }

    private readonly string _channel;
    private readonly int _streamId;
    
    private CancellationTokenSource _cancel;
    private Task _runningTask;

    public DriverActor(string channel, int streamId)
    {
        _channel = channel;
        _streamId = streamId;
        _cancel = new CancellationTokenSource();
        Ready();
    }

    private void Ready()
    {
        Receive<Start>(s =>
        {
            var self = Self; // closure
            _runningTask = Task.Run(() =>
                {
                    // ... work
                }, _cancel.Token).ContinueWith(x =>
                {
                    if (x.IsCanceled) 
                        self.Tell(new Cancelled());
                    else if(x.IsFaulted)
                        self.Tell(new Failed());
                    else 
                        self.Tell(new Finished());
                }, TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(self);

            // switch behavior
            Become(Working);
        });
    }

    private void Working(){
        Receive<Cancel>(cancel => {
            _cancel.Cancel(); // cancel work
            //TODO: should we wait for finish message first?
            BecomeReady();
        });
        Receive<Failed>(f => BecomeReady());
        Receive<Cancelled>(f => BecomeReady());
        Receive<Finished>(f => BecomeReady());
        ReceiveAny(o => Stash.Stash());
    }

    private void BecomeReady(){
        _cancel = new CancellationTokenSource();
        Stash.UnstashAll();
        Become(Ready);
    }
}