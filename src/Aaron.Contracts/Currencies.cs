namespace Aaron.Contracts;

public static class Currencies
{
    public static readonly Currency Eur = new(EUR, "Euro", "€");
    public static readonly Currency Usd = new(USD, "US Dollar", "$");
    public static readonly Currency Gbp = new(GBP, "British Pound", "£");
    public static readonly Currency Dkk = new(DKK, "Danish krone", "kr.");
    public static readonly Currency Pln = new(PLN, "Polish zloty", "zł");
    public static readonly Currency Jpy = new(JPY, "Japanis yen", "¥", 0);
    public static readonly Currency Chf = new(CHF, "Swiss Franc", "₣");


    public const string EUR = "EUR";
    public const string USD = "USD";
    public const string GBP = "GBP";
    public const string DKK = "DKK";
    public const string PLN = "PLN";
    public const string JPY = "JPY";
    public const string CHF = "CHF";
}