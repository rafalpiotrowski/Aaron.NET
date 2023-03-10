using MatchingEngine.Matching;

namespace MatchingEngine;

public class MatchingEngine
{
    private readonly OrderBook _book;
    private readonly MarketOrderMatchingStrategy _marketOrderMatchingStrategy;

    public MatchingEngine(OrderBook book, DefaultObjectPool<Order> orderPool)
    {
        _book = book;
        //todo: initialize all the strategies
        _marketOrderMatchingStrategy = new MarketOrderMatchingStrategy(book, orderPool);
    }

    public void Initialize()
    {
        _book.InitializePool();
    }

    public Order[] Match(Order order)
    {
        switch (order.Type)
        {
            case OrderType.Market:
                return _marketOrderMatchingStrategy.Match(order);
            default:
                throw new NotSupportedException($"{order.Type} order type is not supported");
        }
    }
}

