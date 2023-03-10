namespace MatchingEngine.Matching;

public abstract class MatchingStrategy
{
    protected DefaultObjectPool<Order> OrderPool { get; private set; }
    protected OrderBook OrderBook { get; private set; }

    public MatchingStrategy(OrderBook orderBook, DefaultObjectPool<Order> orderPool)
    {
        OrderPool = orderPool;
        OrderBook = orderBook;
    }
}
