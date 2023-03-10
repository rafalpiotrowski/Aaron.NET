namespace MatchingEngine;

public class PriceLevel
{
    public decimal Price { get; internal set; }
    public readonly OrderList Orders;

    public PriceLevel()
    {
        Debug.Fail("Should not be called; Required by DefaultPooledObjectPolicy");
    }
    public PriceLevel(DefaultObjectPool<OrderList> orderListPool)
    {
        Orders = orderListPool.Get();
    }

    public override int GetHashCode() => Price.GetHashCode();
    public override bool Equals(object? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return Price == ((PriceLevel)obj!).Price;
    }

    public override string ToString()
    {
        return $"PriceLevel {Price} Size: {Orders.Count}";
    }

    public bool CanReturnToPool() => Orders.Count == 0;
}