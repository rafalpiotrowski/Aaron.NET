using Aaron.Contracts;
using ProtoBuf;

namespace Symbology.Contracts;

[ProtoContract]
public sealed record InstrumentId
{
    public required string Value { get; init; }
    public static readonly InstrumentId Empty = new() { Value = string.Empty };
}

[ProtoContract]
public sealed record Instrument : ISymbologyProtocolMember, IWithId<InstrumentId>
{
    public InstrumentId Id { get; init; } = InstrumentId.Empty;
    /// <summary>
    /// Common, "human understood" representation of the security. SecurityId value can be specified
    /// if no symbol exists (e.g. non-exchange traded Collective Investment Vehicles)
    /// Use '[N/A]' for products which do not have a symbol.
    /// </summary>
    public string Symbol { get; init; } = "N/A";
    public SecurityId[] SecurityIds { get; init; } = Array.Empty<SecurityId>();
    public Product Product { get; init; } = Product.Undefined;
}