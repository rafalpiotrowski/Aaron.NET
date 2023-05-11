using ProtoBuf;

namespace Aaron.Contracts.Commands;

[ProtoContract]
public sealed record Start : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}
