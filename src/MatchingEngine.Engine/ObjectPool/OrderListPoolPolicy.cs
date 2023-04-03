namespace MatchingEngine.Engine.ObjectPool;

internal class OrderListPoolPolicy : DefaultPooledObjectPolicy<OrderList>
{
    public override OrderList Create()
    {
        //we can add extra logic here when creating new instance.
        //for example we could 
        return new OrderList();
    }
}
