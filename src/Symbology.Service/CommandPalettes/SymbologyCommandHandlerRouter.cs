using System.Linq.Expressions;
using Akka.Actor;
using Petabridge.Cmd.Host;
using Petabridge.Cmd;

namespace Symbology.Service.CommandPalettes;

/// <summary>
///     INTERNAL API
///     Used to translate <see cref="T:Petabridge.Cmd.Command" />s related to <see cref="T:Symbology" />
///     into discrete, handle-able messages.
/// </summary>
internal sealed class SymbologyCommandHandlerRouter : CommandHandlerActor
{
    private IActorRef _symbologyManager;

    /// <summary>For unit-testing purposes only.</summary>
    /// <param name="symbologyManager"></param>
    public SymbologyCommandHandlerRouter(IActorRef symbologyManager)
        : this()
    {
        _symbologyManager = symbologyManager;
    }

    public SymbologyCommandHandlerRouter()
        : base(SymbologyCmd.SymbologyPalette)
    {
        Receive<Command>(c => c.Name.Equals(SymbologyCmd.AddInstrument.Name),
            c => ExecuteCommand(c, command =>
                _symbologyManager.Forward(SymbologyCmdMsgs.AddInstrument.Instance)));
    }
    
    protected override void PreStart()
    {
        if (_symbologyManager != null)
            return;
        _symbologyManager = Context.ActorOf(
            Props.Create<SymbologyCommandHandlerActor>(() => Expression.New(typeof(SymbologyCommandHandlerActor))),
            "symbology-manager");
    }
}