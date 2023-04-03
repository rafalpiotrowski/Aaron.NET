namespace MatchingEngine.Engine;

public class BuyBook : Book
{
    public BuyBook(DefaultObjectPool<PriceLevel> levelsPool) : base(levelsPool)
    {
    }
    private class PriceComparer : IComparer<decimal>
    {
        public int Compare(decimal x, decimal y)
        {
            return y.CompareTo(x);
        }
    }

    protected override IComparer<decimal> Comparer => new PriceComparer();

    public override decimal QueryNextBestAfter(decimal price, decimal after)
    {
        return base.SortedPrices.FirstOrDefault(p => p < after && p >= price);
    }
}


