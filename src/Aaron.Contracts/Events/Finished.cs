using ProtoBuf;

namespace Aaron.Contracts.Events;

[ProtoContract]
public sealed record Finished : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}