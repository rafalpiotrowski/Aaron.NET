namespace Symbology.Service.CommandPalettes;

internal static class SymbologyCmdMsgs
{
    public sealed class AddInstrument
    {
        public static readonly SymbologyCmdMsgs.AddInstrument Instance = new ();

        private AddInstrument()
        {
        }
    }
}