using ProtoBuf;

namespace Symbology.Contracts;

/// <summary>
/// Indicates the type of product the security is associated with.
/// </summary>
[ProtoContract]
public enum Product
{
    Undefined = 0,
    Agency = 1,
    Commodity = 2, 
    Corporate = 3, 
    Currency = 4,
    Equity = 5,
    Government = 6, 
    Index = 7,
    Loan = 8,
    MoneyMarket = 9,
    Mortgage = 10,
    Municipal = 11,
    Other = 12,
    Financing = 13
}