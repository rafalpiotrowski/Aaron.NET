using ProtoBuf;

namespace Aaron.Contracts;


[ProtoContract(SkipConstructor = false)]
public sealed record class Money : IProtocolMember
{
    public Money()
    {
        Currency = new();
        Precision = 0;
        Amount = decimal.Zero;
    }

    public Money(decimal amount, Currency currency)
    {
        Currency = currency;
        Precision = Currency.Precision;
        Amount = Math.Round(amount, Precision, MidpointRounding.ToEven);
    }
    public Money(decimal amount, Currency currency, int precision)
    {
        Currency = currency;
        Precision = precision <= -1 ? Currency.Precision : precision;
        Amount = Math.Round(amount, Precision, MidpointRounding.ToEven);
    }

    [ProtoMember(1)] public decimal Amount { get; init; }
    [ProtoMember(2)] public Currency Currency { get; init; }
    [ProtoMember(3)] public int Precision { get; init; }

    public static Money Euro(decimal Amount) => new(Amount, Currencies.Eur);
    public static Money UsDollar(decimal Amount) => new(Amount, Currencies.Usd);

    /// <summary>
    /// Divide the Money in equal shares, without losing Money.
    /// </summary>
    /// <param name="shares"></param>
    /// <param name="decimalDigits">default 2 decimal places</param>
    /// <returns></returns>
    public Money[] Allocate(int shares, int decimalDigits = -1)
    {
        int precision = decimalDigits <= -1 ? Currency.Precision : decimalDigits;
        decimal shareAmount = Math.Round(Amount / shares, precision, MidpointRounding.ToEven);
        decimal remainder = Amount;

        var shareResult = this with { Amount = shareAmount };
        Money[] results = new Money[shares];
        int i = 0;
        for (; i < shares - 1; i++)
        {
            results[i] = shareResult;
            remainder -= shareAmount;
        }
        results[i] = this with { Amount = remainder };
        return results;
    }

    /// <summary>
    /// Allocate the Money in shares with a specific ratio, without losing Money.
    /// </summary>
    /// <param name="ratios">list of % hot to allocate the amount e.g. {4, 6}</param>
    /// <param name="decimalDigits">if not specified use default of the Currency.Precision</param>
    /// <returns></returns>
    public Money[] Allocate(int[] ratios, int decimalDigits = -1)
    {
        if (ratios.Any(ratio => ratio < 1))
            throw new ArgumentOutOfRangeException(nameof(ratios), "All ratios must be greater or equal than 1");

        int precision = decimalDigits <= -1 ? Precision : decimalDigits;
        int total = ratios.Sum();
        decimal remainder = Amount;
        Money[] results = new Money[ratios.Length];
        int i = 0;
        for (; i < results.Length - 1; i++)
        {
            results[i] = this with { Amount = Math.Round(Amount * ratios[i] / total, precision, MidpointRounding.ToEven) };
            remainder -= results[i].Amount;
        }
        results[i] = this with { Amount = remainder };
        return results;
    }

    public static Money operator -(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m with { Amount = m.Amount - m1.Amount };
        throw new ArgumentException("Money currency must be the same");
    }
    public static Money operator +(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m with { Amount = m.Amount + m1.Amount };
        throw new ArgumentException("Money currency must be the same");
    }
    public static bool operator <(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m.Amount < m1.Amount;
        throw new ArgumentException("Money currency must be the same");
    }
    public static bool operator >(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m.Amount > m1.Amount;
        throw new ArgumentException("Money currency must be the same");
    }
    public static bool operator <=(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m.Amount <= m1.Amount;
        throw new ArgumentException("Money currency must be the same");
    }
    public static bool operator >=(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m.Amount >= m1.Amount;
        throw new ArgumentException("Money currency must be the same");
    }

    public static Money operator *(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m with { Amount = Math.Round(m.Amount * m1.Amount, m.Precision, MidpointRounding.ToEven) };
        throw new ArgumentException("Money currency must be the same");
    }

    public static Money operator /(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return m with { Amount = Math.Round(m.Amount / m1.Amount, m.Precision, MidpointRounding.ToEven) };
        throw new ArgumentException("Money currency must be the same");
    }

    public static Money operator /(Money m, ExchangeRate rate) => rate.Convert(m);
    public static Money operator *(Money m, ExchangeRate rate) => rate.Convert(m);

    public static Money operator -(Money m, decimal amount) => m with { Amount = m.Amount - amount };

    public static Money operator +(Money m, decimal amount) => m with { Amount = m.Amount + amount };
    public static Money operator *(Money m, decimal amount) => m with { Amount = Math.Round(m.Amount * amount, m.Precision, MidpointRounding.ToEven) };
    public static Money operator /(Money m, decimal amount) => m with { Amount = Math.Round(m.Amount / amount, m.Precision, MidpointRounding.ToEven) };

    public static bool operator <(Money m, decimal amount) => m.Amount < amount;
    public static bool operator >(Money m, decimal amount) => m.Amount > amount;
    public static bool operator <=(Money m, decimal amount) => m.Amount <= amount;
    public static bool operator >=(Money m, decimal amount) => m.Amount >= amount;

    public static bool operator ==(Money m, decimal amount) => m.Amount == amount;
    public static bool operator !=(Money m, decimal amount) => m.Amount != amount;

    public static decimal? Abs(Money m, Money m1)
    {
        if (m.Currency.Code == m1.Currency.Code)
            return Math.Abs(m.Amount - m1.Amount);
        throw new ArgumentException("Money currency must be the same");
    }

    /// <summary>
    /// this.Amount = 100
    /// this.Currency = EUR
    /// rate.BaseCurrency = EUR
    /// rate.QuoteCurrency = USD
    /// rate.Rate = 0.9
    /// we will get Money(90, USD)
    /// 
    /// this.Amount = 90
    /// this.Currency = USD
    /// rate.BaseCurrency = EUR
    /// rate.QuoteCurrency = USD
    /// rate.Rate = 0.9
    /// we will get Money(100, EUR)
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Money Exchange(ExchangeRate rate)
    {
        if (Currency.Code == rate.BaseCurrency.Code)
            return this with { Amount = Math.Round(Amount * rate.Rate, Precision, MidpointRounding.ToEven), Currency = rate.QuoteCurrency };
        if (Currency.Code == rate.QuoteCurrency.Code)
            return this with { Amount = Math.Round(Amount / rate.Rate, Precision, MidpointRounding.ToEven), Currency = rate.BaseCurrency };

        throw new ArgumentException("Money should have the same Currency as BaseCurrency or QuoteCurrency!", nameof(rate));
    }
}

public static class MoneyMathExtensions
{
    public static Money Round(this Money money, int precision) => new(money.Amount, money.Currency, precision);
}