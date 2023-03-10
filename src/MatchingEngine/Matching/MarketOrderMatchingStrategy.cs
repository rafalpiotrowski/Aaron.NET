namespace MatchingEngine.Matching;

public class MarketOrderMatchingStrategy : MatchingStrategy
{
    public MarketOrderMatchingStrategy(OrderBook orderBook, DefaultObjectPool<Order> orderPool) : base(orderBook, orderPool)
    {
    }
    
    public Order[] Match(Order order)
    {
        //this is market type order so we do not care what price we fill
        //we have to start with best price and keep filling
        switch (order.Side)
        {
            case OrderSide.Sell:
                return Match(OrderBook.BuyBook, order);
            case OrderSide.Buy:
                return Match(OrderBook.SellBook, order);
            default:
                throw new ArgumentOutOfRangeException(nameof(order.Side), order.Side, "not supported");
        }
    }

    private Order[] Match(Book book, Order order)
    {
        var bestPrice = book.QueryFirstBest(order.Price);
        var orders = new List<Order>();
        while (bestPrice != decimal.Zero)
        {
            var matchedOrders = Match(book, order, bestPrice);
            orders.AddRange(matchedOrders);
            if (order.Filled)
            {
                break;
            }
            bestPrice = book.QueryNextBestAfter(order.Price, bestPrice);
        } 
        if (!order.Filled)
        {
            //place the remaining of the order as limit order
            var newOrder = OrderPool.Get();
            newOrder.OrderId = order.OrderId;
            newOrder.Price = order.Price;
            newOrder.Quantity = order.Quantity - order.FilledQuantity;
            newOrder.Side = order.Side;
            // market order becomes limit order, since we filled what we could, this
            // would happen when there are no other orders to match
            newOrder.Type = OrderType.Limit;
            //we can safely add it since there is no matching order left
            OrderBook.Add(newOrder); 
        }
        return orders.ToArray();
    }
    
    private List<Order> Match(Book book, Order order, decimal levelPrice)
    {
        try
        {
            var l = CollectionsMarshal.GetValueRefOrNullRef(book.Levels, levelPrice);
            var list = new List<Order>();
            var remaining = order.Quantity - order.FilledQuantity;
            while (true)
            {
                var fillingOrder = l.Orders.RemoveFirst();
                if (remaining >= fillingOrder.Quantity)
                {
                    fillingOrder.Filled = true;
                    fillingOrder.FilledQuantity = fillingOrder.Quantity;
                    fillingOrder.FilledPrice = order.Price;
                    fillingOrder.FilledOrderId = order.OrderId;
                    remaining -= fillingOrder.Quantity;
                    order.FilledQuantity += fillingOrder.Quantity;
                    if (remaining == 0)
                    {
                        order.Filled = true;
                    }
                }
                else // if(remaining < fillingOrder.Quantity)
                {
                    //remaining -= fillingOrder.Quantity;
                    fillingOrder.Filled = false;
                    fillingOrder.FilledQuantity = remaining;
                    fillingOrder.FilledPrice = order.Price;
                    fillingOrder.FilledOrderId = order.OrderId;
                    remaining = 0;
                    order.FilledQuantity += remaining;
                    order.Filled = true;
                }
                list.Add(fillingOrder);
                if (l.Orders.Count == 0)
                {
                    book.Clean(order.Price, l);
                    break;
                }
                else if (order.Filled)
                {
                    break;
                }
            }

            return list;
        }
        catch (NullReferenceException)
        {
            //can be thrown by CollectionsMarshal.GetValueRefOrNullRef
            //todo: throw our own exception
            throw;
        }
    }
}







