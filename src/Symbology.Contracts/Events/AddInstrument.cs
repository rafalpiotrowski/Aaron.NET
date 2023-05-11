using Aaron.Contracts;
using ProtoBuf;

namespace Symbology.Contracts.Events;

[ProtoContract]
public sealed record AddInstrumentOk : ISymbologyEvent
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
    public required InstrumentId InstrumentId { get; init; }
}

[ProtoContract]
public sealed record AddInstrumentFailed : ISymbologyEvent
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
    public required Instrument Instrument { get; init; }
    public required AddInstrumentFailureReason Reason { get; init; }
}

[ProtoContract]
public sealed record AddInstrumentFailureReason
{
    public required int Code { get; init; }
    public required string Description { get; init; }

    public static readonly AddInstrumentFailureReason AlreadyExists = new()
        { Code = 1, Description = "Instrument already exists" };
    public static readonly AddInstrumentFailureReason InvalidSpecification = new()
        { Code = 2, Description = "Instrument incorrectly defined" };
}