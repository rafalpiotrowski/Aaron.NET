using ProtoBuf;

namespace Aaron.Contracts.Commands;

[ProtoContract]
public sealed record Cancel : IMessage
{
    public MessageHeader Header { get; init; } = MessageHeader.Empty;
}