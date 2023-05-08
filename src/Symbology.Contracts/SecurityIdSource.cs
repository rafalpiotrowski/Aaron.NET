using ProtoBuf;

namespace Symbology.Contracts;

[ProtoContract]
public enum SecurityIdSource
{
    Undefined = 0,
    Cusip = 1,
    Sedol = 2,
    Quick = 3,
    Isin = 4, 
    Ric = 5, 
    ExchangeSymbol = 8,
    Bloomberg = 'A',
}