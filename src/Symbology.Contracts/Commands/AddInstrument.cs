using Aaron.Contracts;
using ProtoBuf;

namespace Symbology.Contracts.Commands;

[ProtoContract]
public sealed record AddInstrument : ISymbologyCommand, IWithId<InstrumentId>
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
    public required Instrument Instrument { get; init; }
    public InstrumentId Id => Instrument.Id;
}