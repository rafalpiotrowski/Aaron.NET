using ProtoBuf;

namespace Aaron.Contracts;


[ProtoContract(SkipConstructor = false)]
public sealed record class  ExchangeRate : IProtocolMember
{
    public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate)
    {
        if (rate < 0) throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be greater than 0");
        if (baseCurrency.Code == string.Empty) throw new ArgumentOutOfRangeException(nameof(baseCurrency), "Base currency must be specified!");
        if (quoteCurrency.Code == string.Empty) throw new ArgumentOutOfRangeException(nameof(quoteCurrency), "Quote currency must be specified!");
        if (baseCurrency.Code == quoteCurrency.Code && rate != decimal.One)
        {
            throw new ArgumentException($"When Base and Quote currency are the same, Rate must be 1!");
        }
        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        Rate = rate;
    }
    public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate, int precision = 8)
        : this(baseCurrency, quoteCurrency, Math.Round(rate, precision))
    {
        if (precision < 0)
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be greater than 0");
        Precision = precision;
    }

    public ExchangeRate()
    {
        BaseCurrency = new();
        QuoteCurrency = new();
        Rate = decimal.Zero;
        Precision = 0;
    }

    [ProtoMember(1)] public Currency BaseCurrency { get; init; }
    [ProtoMember(2)] public Currency QuoteCurrency { get; init; }
    [ProtoMember(3)] public decimal Rate { get; init; }
    [ProtoMember(4)] public int Precision { get; init; }

    [ProtoIgnore] public string PairCode => $"{BaseCurrency.Code}{QuoteCurrency.Code}";

    public static ExchangeRate None(Currency ccy) => new(ccy, ccy, decimal.One);

    /// <summary>
    /// e.g. 
    /// BaseCurrency = EUR
    /// QuoteCurrency = PLN
    /// Rate = 4.50
    /// Convert(Money.Euro(10)) => 
    ///     EUR == BaseCurrency => 10 * 4.50 => 45 PLN
    ///     
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Money Convert(Money money)
    {
        if(money.Currency != BaseCurrency && money.Currency != QuoteCurrency)
        {
            throw new ArgumentException("Money should have the same Currency as BaseCurrency or QuoteCurrency!", nameof(money));
        }

        return money.Currency == BaseCurrency
            ? money with { Amount = Math.Round(money.Amount * Rate, QuoteCurrency.Precision, MidpointRounding.ToEven), Currency = QuoteCurrency }
            : money with { Amount = Math.Round(money.Amount / Rate, BaseCurrency.Precision, MidpointRounding.ToEven), Currency = BaseCurrency };
    }
}
