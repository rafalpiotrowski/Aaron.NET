using ProtoBuf;

namespace MatchingEngine.Contracts;

[ProtoContract]
public sealed record OrderMatched : IProtocolMember
{
    public required long OrderId { get; init; }
    public required int Units { get; init; }
    public required decimal Price { get; init; }
}