using MatchingEngine.Engine;

namespace MatchingEngine.Tests;

public class MatchingEngingUnitTests : IDisposable
{
    private readonly Engine.Engine _engine;
    private readonly DefaultObjectPool<Order> _orderPool;

    public MatchingEngingUnitTests()
    {
        _orderPool = new(new OrderPoolPolicy(), 10);
        OrderBook book = new(10);
        _engine = new Engine.Engine(book, _orderPool);
        _engine.Initialize();
    }
    
    public void Dispose()
    {
    }

    [Fact]
    public void Match_AllOrders_EmptyOrderBook_Success()
    {
        _engine.Match(CreateOrder(1, OrderSide.Sell, 100.10M, 200));
        _engine.Match(CreateOrder(2, OrderSide.Sell, 100.10M, 400));
        _engine.Match(CreateOrder(3, OrderSide.Buy, 100.10M, 600));

        Assert.Empty(_engine.Book.BuyBook.Levels);
        Assert.Empty(_engine.Book.SellBook.Levels); 
    }

    private Order CreateOrder(long orderId, OrderSide side, decimal price, int quantity, OrderType type = OrderType.Market)
    {
        var order = _orderPool.Get();
        order.OrderId = orderId;
        order.Side = side;
        order.Price = price;
        order.Quantity = quantity;
        order.Type = type;
        return order;
    }


}