namespace MatchingEngine.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        DefaultObjectPool<Order> orderPool = new(new OrderPoolPolicy(), 100);

        OrderBook book = new();
        var engine = new MatchingEngine(book, orderPool);

        Order[] matchOrders = null;

        matchOrders = engine.Match(CreateOrder(1, Order.OrderSide.Sell, 100.10M, 200));
        matchOrders = engine.Match(CreateOrder(2, Order.OrderSide.Sell, 100.10M, 400));
        matchOrders = engine.Match(CreateOrder(3, Order.OrderSide.Sell, 100.10M, 1100));
        matchOrders = engine.Match(CreateOrder(4, Order.OrderSide.Sell, 100.10M, 100));
        matchOrders = engine.Match(CreateOrder(5, Order.OrderSide.Sell, 100.11M, 900));
        matchOrders = engine.Match(CreateOrder(6, Order.OrderSide.Sell, 100.12M, 600));
        matchOrders = engine.Match(CreateOrder(7, Order.OrderSide.Buy, 100.10M, 2700));
        matchOrders = engine.Match(CreateOrder(8, Order.OrderSide.Buy, 100M, 10000));

        Assert.True(true);
        
        Order CreateOrder(long orderId, Order.OrderSide side, decimal price, int quantity, Order.OrderType type = Order.OrderType.Market)
        {
            var order = orderPool.Get();
            order.OrderId = orderId;
            order.Side = side;
            order.Price = price;
            order.Quantity = quantity;
            order.Type = type;
            return order;
        }
    }
}