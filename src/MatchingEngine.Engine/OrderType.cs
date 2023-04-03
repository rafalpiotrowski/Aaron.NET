namespace MatchingEngine.Engine;

public enum OrderType
{
    ///A Market execution instruction is a type of order that instructs the broker or exchange to execute the order at the best available market price at the time the order is received.
    Market = 0,
    Limit
}