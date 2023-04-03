namespace MatchingEngine.Engine.ObjectPool;

internal class PriceLevelPoolPolicy : DefaultPooledObjectPolicy<PriceLevel>
{
    private readonly DefaultObjectPool<OrderList> _orderListPool;

    public PriceLevelPoolPolicy(int poolSize) 
    {
        _orderListPool = new(new OrderListPoolPolicy(), poolSize);
        var pool = new OrderList[poolSize];
        for(var i = 0; i<pool.Length; i++)
        {
            pool[i] = _orderListPool.Get();
        }
        foreach (var t in pool)
        {
            _orderListPool.Return(t);
        }
        Array.Clear(pool);
        pool = null;
    }

    public override PriceLevel Create()
    {
        //we can add extra logic here when creating new instance.
        //for example we could 
        return new PriceLevel(_orderListPool);
    }
}