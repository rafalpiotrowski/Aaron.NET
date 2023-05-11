using ProtoBuf;

namespace Aaron.Contracts.Events;

[ProtoContract]
public sealed record Cancelled : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}