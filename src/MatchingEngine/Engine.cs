using MatchingEngine.Matching;

namespace MatchingEngine;

public class MatchingEngine
{
    private readonly MarketOrderMatchingStrategy _marketOrderMatchingStrategy;

    public MatchingEngine(OrderBook book, DefaultObjectPool<Order> orderPool)
    {
        //todo: initialize all the strategies
        _marketOrderMatchingStrategy = new MarketOrderMatchingStrategy(book, orderPool);
    }

    public Order[] Match(Order order)
    {
        switch (order.Type)
        {
            case Order.OrderType.Market:
                return _marketOrderMatchingStrategy.Match(order);
                break;
            default:
                throw new NotSupportedException($"{order.Type} order type is not supported");
        }
    }
}

