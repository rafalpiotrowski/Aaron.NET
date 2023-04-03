namespace Aaron.Contracts;

public static class Countries
{
    public static readonly Country Italy = new(ITA, "Italy", Currencies.Eur);
    public static readonly Country Uk = new(GBR, "United Kingdom", Currencies.Gbp);
    public static readonly Country Poland = new(POL, "Poland", Currencies.Pln);
    public static readonly Country Switzerland = new(CH, "Switzerland", Currencies.Chf);

    public const string ITA = "ITA";
    public const string GBR = "GBP";
    public const string POL = "POL";
    public const string CH = "CH";
}