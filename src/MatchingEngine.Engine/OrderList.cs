namespace MatchingEngine.Engine;

public class OrderList
{
    private readonly LinkedList<Order> _orders;

    public OrderList()
    {
        _orders = new LinkedList<Order>();
    }

    public void Add(Order order)
    {
        _orders.AddLast(order);
    }

    public int Count => _orders.Count;

    /// <summary>
    /// call to Count should be made first 
    /// </summary>
    /// <returns></returns>
    public Order RemoveFirst()
    {
        var order = _orders.First!.Value;
        _orders.RemoveFirst();
        return order;
    }
}

