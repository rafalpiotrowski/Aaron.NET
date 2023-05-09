using Petabridge.Cmd;

namespace Symbology.Service.CommandPalettes;

internal static class SymbologyCmd
  {
    public static readonly CommandDefinition AddInstrument = new CommandDefinitionBuilder().WithName("add").WithDescription("Add instrument").Build();
    public static readonly CommandPalette SymbologyPalette = new CommandPalette("symbology", (IEnumerable<CommandDefinition>) new CommandDefinition[1]
    {
      SymbologyCmd.AddInstrument,
    });
  }