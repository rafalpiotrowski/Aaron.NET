using System.Collections.Immutable;
using ProtoBuf;

namespace Aaron.Contracts;

[ProtoContract]
public sealed record MessageHeader
{
    /// <summary>
    /// Message sequence number, assigned by the <see cref="Aaron.Contracts.ISequencer"/>
    /// </summary>
    public required ulong SequenceNr { get; init; }
    public string TraceId { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> Values { get; init; } = ImmutableDictionary<string, string>.Empty;
    public static readonly MessageHeader Empty = new() { SequenceNr = 0 };
}