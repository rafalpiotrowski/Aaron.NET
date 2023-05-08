using ProtoBuf;

namespace Symbology.Contracts;

[ProtoContract]
public sealed record SecurityId 
{
    public required string Id { get; init; }
    public required SecurityIdSource Source { get; init; }
}