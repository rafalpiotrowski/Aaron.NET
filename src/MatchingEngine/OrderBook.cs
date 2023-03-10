using MatchingEngine.ObjectPool;

namespace MatchingEngine;

public class OrderBook
{
    private readonly int _poolSize;
    private readonly DefaultObjectPool<PriceLevel> _levelsPool;
    private bool IsPoolInitialized { get; set; }

    public SellBook SellBook { get; }
    public BuyBook BuyBook { get; }

    public OrderBook(int poolSize)
    {
        _poolSize = poolSize;
        _levelsPool = new(new PriceLevelPoolPolicy(_poolSize), _poolSize);
        SellBook = new(_levelsPool);
        BuyBook = new(_levelsPool);
    }

    public void InitializePool()
    {
        if (IsPoolInitialized) return;
        var pool = new PriceLevel[_poolSize];
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
        IsPoolInitialized = true;
    }

    public void Add(Order order)
    {
        switch (order.Side)
        {
            case OrderSide.Sell:
                SellBook.Add(order);
                break;
            case OrderSide.Buy:
                BuyBook.Add(order);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}