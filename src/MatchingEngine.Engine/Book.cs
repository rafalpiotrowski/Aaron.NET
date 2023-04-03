namespace MatchingEngine.Engine;

public abstract class Book
{
    private readonly DefaultObjectPool<PriceLevel> _levelsPool;
    public Dictionary<decimal, PriceLevel> Levels { get; private set; } 
    protected SortedSet<decimal> SortedPrices { get; private set; }
    
    protected abstract IComparer<decimal> Comparer { get; }
    protected Book(DefaultObjectPool<PriceLevel> levelsPool)
    {
        var comparer = Comparer;
        Debug.Assert(comparer != null, nameof(comparer) + " != null");
        _levelsPool = levelsPool;
        Levels = new();
        SortedPrices = new SortedSet<decimal>(comparer);
    }

    public void Add(Order order)
    {
        var l = CollectionsMarshal.GetValueRefOrAddDefault(Levels, order.Price, out bool exists);
        if(!exists)
        {
            l = _levelsPool.Get();
            Levels[order.Price] = l;
            SortedPrices.Add(order.Price);
        }
        l!.Price = order.Price;
        l.Orders.Add(order);
    }

    /// <summary>
    /// remove first order from the price level
    /// if the list is empty it return it back to the pool and remove the price level from the book
    /// </summary>
    /// <param name="price"></param>
    /// <returns></returns>
    public Order Remove(decimal price)
    {
        var l = Levels[price];
        var order = l.Orders.RemoveFirst();
        if (l.Orders.Count == 0)
        {
            Clean(price, l);
        }
        return order;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clean(decimal price, PriceLevel l)
    {
        _levelsPool.Return(l);
        Levels.Remove(price);
        SortedPrices.Remove(price);
    }

    public abstract decimal QueryNextBestAfter(decimal price, decimal after);
    
    /// <summary>
    /// return first from the set
    /// </summary>
    /// <param name="price"></param>
    /// <returns>decimal.Zero if not found</returns>
    public decimal QueryFirstBest(decimal price)
    {
        if (SortedPrices.Count != 0)
            return SortedPrices.First();
        return decimal.Zero;
    }
}
