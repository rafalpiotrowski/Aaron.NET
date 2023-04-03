namespace MatchingEngine.Engine;

public class SellBook : Book
{
    public SellBook(DefaultObjectPool<PriceLevel> levelsPool) : base(levelsPool)
    {

    }

    private class PriceComparer : IComparer<decimal>
    {
        public int Compare(decimal x, decimal y)
        {
            return x.CompareTo(y);
        }
    }

    protected override IComparer<decimal> Comparer => new PriceComparer();
    
    public override decimal QueryNextBestAfter(decimal price, decimal after)
    {
        return base.SortedPrices.FirstOrDefault(p => p > after && p <= price);
    }
}