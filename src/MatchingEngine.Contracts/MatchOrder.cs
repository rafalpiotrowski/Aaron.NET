using Aaron.Contracts;
using ProtoBuf;

namespace MatchingEngine.Contracts;

[ProtoContract]
public sealed record MatchOrder : IProtocolMember, IWithEngineId
{
    public required string EngineId { get; init; }
    public required long OrderId { get; init; }
    public required Side Side  { get; init; }
    public required int Units { get; init; }
    public required decimal Price { get; init; }
}