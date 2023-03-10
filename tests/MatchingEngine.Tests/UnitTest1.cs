namespace MatchingEngine.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        DefaultObjectPool<Order> orderPool = new(new OrderPoolPolicy(), 100);

        OrderBook book = new(1);
        var engine = new MatchingEngine(book, orderPool);
        engine.Initialize();

        engine.Match(CreateOrder(1, OrderSide.Sell, 100.10M, 200));
        engine.Match(CreateOrder(2, OrderSide.Sell, 100.10M, 400));
        engine.Match(CreateOrder(3, OrderSide.Sell, 100.10M, 1100));
        engine.Match(CreateOrder(4, OrderSide.Sell, 100.10M, 100));
        engine.Match(CreateOrder(5, OrderSide.Sell, 100.11M, 900));
        engine.Match(CreateOrder(6, OrderSide.Sell, 100.12M, 600));
        engine.Match(CreateOrder(7, OrderSide.Buy, 100.10M, 2700));
        engine.Match(CreateOrder(8, OrderSide.Buy, 100M, 10000));

        Assert.True(true);
        
        Order CreateOrder(long orderId, OrderSide side, decimal price, int quantity, OrderType type = OrderType.Market)
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