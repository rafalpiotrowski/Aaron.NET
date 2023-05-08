using ProtoBuf;

namespace Aaron.Contracts;

[ProtoContract]
public sealed record DecimalValue
{
    private const decimal NanoFactor = 1_000_000_000;

    public required long Units { get; init; }
    public required int Nanos { get; init; }
    
    public decimal ToDecimal()
    {
        return Units + Nanos / NanoFactor;
    }

    public static DecimalValue FromDecimal(decimal value)
    {
        var units = decimal.ToInt64(value);
        var nanos = decimal.ToInt32((value - units) * NanoFactor);

        return new DecimalValue { Units = units, Nanos = nanos };
    }

    public static implicit operator decimal(DecimalValue value) => value.ToDecimal();

    public static implicit operator DecimalValue(decimal value) => FromDecimal(value);
}