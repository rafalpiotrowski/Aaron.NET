using MatchingEngine.ObjectPool;

namespace MatchingEngine;

public class OrderBook
{
    private const int PoolSize = 100;
    private readonly DefaultObjectPool<PriceLevel> _levelsPool;

    public SellBook SellBook { get; }
    public BuyBook BuyBook { get; }

    public OrderBook()
    {
        _levelsPool = new(new PriceLevelPoolPolicy(PoolSize), PoolSize);
        
        var pool = new PriceLevel[PoolSize];
        for(var i = 0; i<pool.Length; i++)
        {
            pool[i] = _levelsPool.Get();
        }
        foreach (var t in pool)
        {
            _levelsPool.Return(t);
        }
        Array.Clear(pool);
        pool = null;
        
        SellBook = new(_levelsPool);
        BuyBook = new(_levelsPool);
    }

    public void Add(Order order)
    {
        switch (order.Side)
        {
            case Order.OrderSide.Sell:
                SellBook.Add(order);
                break;
            case Order.OrderSide.Buy:
                BuyBook.Add(order);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}