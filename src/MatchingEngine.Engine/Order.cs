namespace MatchingEngine.Engine;


public class Order
{
    public long OrderId;
    public OrderType Type;
    public OrderSide Side;
    public decimal Price;
    public int Quantity;
    public long SeqId; //sequenceid which should always be incremental 

    public decimal AvgPrice;
    public int FilledQuantity;
    public bool Filled;
    public decimal FilledPrice;
    public long FilledOrderId;

    // public enum ExecutionInstruction
    // {
    //     ///Fill or Kill (FOK): This is an instruction to fill the entire order immediately at the specified price or better, or cancel the entire order if it cannot be filled immediately.
    //     FillOrKill,
    //     ///Immediate or Cancel (IOC): This is an instruction to fill as much of the order as possible immediately at the specified price or better, and cancel any portion of the order that cannot be filled immediately.
    //     ImmediateOrCancel,
    //     ///Good 'Til Canceled (GTC): This is an instruction to keep the order open and working until it is either filled or canceled by the trader.
    //     GoodTillCancelled,
    //     ///All or None (AON): This is an instruction to fill the entire order in a single transaction or not at all.
    //     AllOrNone,
    //     ///Market-on-Close (MOC): This is an instruction to execute the order at the market price at the end of the trading day.
    //     MarketOnClose,
    //     ///Limit-on-Close (LOC): This is an instruction to execute the order at the limit price or better at the end of the trading day.
    //     LimitOnClose,
    //     ///Stop order: This is an instruction to execute a trade when the price of a security reaches a certain level, known as the stop price. Once the stop price is reached, the order is executed at the market price.
    //     StopOrder
    // }
}