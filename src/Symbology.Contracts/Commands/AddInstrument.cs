using Aaron.Contracts;
using ProtoBuf;

namespace Symbology.Contracts.Commands;

[ProtoContract]
public sealed record AddInstrument : ICommand
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
    public required Instrument Instrument { get; init; }
}