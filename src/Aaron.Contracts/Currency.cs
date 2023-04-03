using ProtoBuf;

namespace Aaron.Contracts;


[ProtoContract(SkipConstructor = false)]
public sealed record class Currency : IProtocolMember
{
    [ProtoMember(1)] public string Code { get; init; }
    [ProtoMember(2)] public string Name { get; init; }
    [ProtoMember(3)] public string Symbol { get; init; }
    [ProtoMember(4)] public int Precision { get; init; } = 6;

    public Currency()
    {
        Code = string.Empty;
        Name = string.Empty;
        Symbol = string.Empty;
        Precision = 0;
    }

    public Currency(string code, string name, string symbol, int precision = 6)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        Precision = precision;
    }

    /// <summary>
    /// maps EUR, USD, GBP
    /// for others exception is raised
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Currency From(string currencyCode)
        => currencyCode switch
        {
            Currencies.EUR => Currencies.Eur,
            Currencies.USD => Currencies.Usd,
            Currencies.GBP => Currencies.Gbp,
            Currencies.DKK => Currencies.Dkk,
            Currencies.PLN => Currencies.Pln,
            Currencies.JPY => Currencies.Jpy,
            Currencies.CHF => Currencies.Chf,
            _ => throw new ArgumentOutOfRangeException($"Unsupported currency '{currencyCode}"),
        };

    /// <summary>
    /// if not matched then returns new Currency(currencyCode,currencyCode,currencyCode)
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <param name="ccy"></param>
    /// <returns></returns>
    public static bool TryFrom(string currencyCode, out Currency ccy)
    {
        switch (currencyCode)
        {
            case Currencies.EUR:
                {
                    ccy = Currencies.Eur;
                    return true;
                }
            case Currencies.USD:
                {
                    ccy = Currencies.Usd;
                    return true;
                }
            case Currencies.GBP:
                {
                    ccy = Currencies.Gbp;
                    return true;
                }
            case Currencies.DKK:
                {
                    ccy = Currencies.Dkk;
                    return true;
                }
            case Currencies.PLN:
                {
                    ccy = Currencies.Pln;
                    return true;
                }
            case Currencies.JPY:
                {
                    ccy = Currencies.Jpy;
                    return true;
                }
            case Currencies.CHF:
                {
                    ccy = Currencies.Chf;
                    return true;
                }
            default:
                {
                    ccy = new(currencyCode, currencyCode, currencyCode);
                    return false;
                }
        }
    }
}
