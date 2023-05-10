using ProtoBuf;

namespace Aaron.Contracts.Events;

[ProtoContract]
public sealed record Cancel : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}