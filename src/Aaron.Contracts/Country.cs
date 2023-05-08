using ProtoBuf;

namespace Aaron.Contracts;

[ProtoContract(SkipConstructor = false)]
public sealed record class Country : IProtocolMember
{
    [ProtoMember(1)] public string Code { get; init; }
    [ProtoMember(2)] public string Name { get; init; }
    [ProtoMember(3)] public Currency Currency { get; init; }

    public Country() : this(string.Empty, string.Empty, new())
    {
    }

    public Country(string code, string name, Currency currency)
    {
        Code = code;
        Name = name;
        Currency = currency;
    }

    /// <summary>
    /// maps ITA, GBR, POL
    /// for others exception is raised
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Country From(string code)
        => code switch
        {
            Countries.ITA => Countries.Italy,
            Countries.GBR => Countries.Uk,
            Countries.POL => Countries.Poland,
            Countries.CH => Countries.Switzerland,
            _ => throw new ArgumentOutOfRangeException($"Unsupported country '{code}"),
        };

    /// <summary>
    /// if not matched then returns new Country(code,code,new())
    /// </summary>
    /// <param name="code"></param>
    /// <param name="country"></param>
    /// <returns></returns>
    public static bool TryFrom(string code, out Country country)
    {
        switch (code)
        {
            case Countries.ITA:
                {
                    country = Countries.Italy;
                    return true;
                }
            case Countries.GBR:
                {
                    country = Countries.Uk;
                    return true;
                }
            case Countries.POL:
                {
                    country = Countries.Poland;
                    return true;
                }
            case Countries.CH:
                {
                    country = Countries.Switzerland;
                    return true;
                }
            default:
                {
                    country = new(code, code, new());
                    return false;
                }
        }
    }
}
