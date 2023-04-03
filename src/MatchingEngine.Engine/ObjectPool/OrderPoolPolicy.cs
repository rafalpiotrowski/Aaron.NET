namespace MatchingEngine.Engine.ObjectPool;


public class OrderPoolPolicy : DefaultPooledObjectPolicy<Order>
{
    public override Order Create()
    {
        //we can add extra logic here when creating new instance.
        //for example we could 
        var order = new Order
        {
            Filled = false
        };
        return order;
    }

    /// <summary>
    /// Returns an order to the pool
    /// </summary>
    /// <param name="order">object to be returned</param>
    /// <returns>true</returns>
    /// <exception cref="Exception">if order.Filled != false </exception>
    public override bool Return(Order order)
    {
        if (!order.Filled)
        {
            throw new Exception("Only filled orders can be returned to the pool");
        }
        
        order.Price = decimal.Zero;
        order.Quantity = 0;
        order.Filled = false;
        order.FilledQuantity = 0;
        order.AvgPrice = decimal.Zero;
        order.FilledOrderId = 0;
        order.OrderId = 0;
        
        return base.Return(order);
    }
}