using ProtoBuf;

namespace Aaron.Contracts.Events;

[ProtoContract]
public sealed record Failed : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}