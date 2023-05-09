using System.Linq.Expressions;
using Akka.Actor;
using Petabridge.Cmd.Host;
#pragma warning disable CS8618

namespace Symbology.Service.CommandPalettes;

public class SymbologyCommands : CommandPaletteHandler
{
    /// <summary>Singleton instance of this palette</summary>
    public static readonly SymbologyCommands Instance = new();

    private SymbologyCommands() : base(SymbologyCmd.SymbologyPalette)
    {
    }
    
    private Props _underlyingProps;
    public override Props HandlerProps => _underlyingProps;

    /*
     * Overriding this method gives us the ability to do things like create the SymbologyManagerActor before HandlerProps gets used
     */
    public override void OnRegister(PetabridgeCmd plugin)
    {
        var router = plugin.Sys.ActorOf(Props.Create(() => new SymbologyCommandHandlerActor()), "symbology-mamager");

        // will be used to create a new MsgCommandHandlerActor instance per connection
        _underlyingProps = Props.Create(() => new SymbologyCommandHandlerRouter(router));
        base.OnRegister(plugin);
    }
}