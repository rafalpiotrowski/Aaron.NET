using Akka.Actor;
using Petabridge.Cmd;

namespace Symbology.Service.CommandPalettes;

public sealed class SymbologyCommandHandlerActor : ReceiveActor
{
    public SymbologyCommandHandlerActor()
    {
        Receive<SymbologyCmdMsgs.AddInstrument>(i =>
        {
            Sender.Tell(new CommandResponse("instrument add processed"));
        });
    }
    
}