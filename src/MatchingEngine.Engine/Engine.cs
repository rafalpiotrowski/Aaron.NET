using MatchingEngine.Engine.Matching;

namespace MatchingEngine.Engine;

public class Engine
{
    private readonly OrderBook _book;
    public OrderBook Book => _book;
    private readonly MarketOrderMatchingStrategy _marketOrderMatchingStrategy;

    public Engine(OrderBook book, DefaultObjectPool<Order> orderPool)
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

