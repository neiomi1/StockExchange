namespace StockExchange.Models
{
    public class Trade : IBaseEntity
    {
        public int Id { get; set; }
        public required ICollection<StockShare> StockShares { get; set; }
        public required Trader Seller { get; set; }
        public required Trader Buyer { get; set; }
        public decimal TradePrice { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public Trade() { }

        public Trade CombineTrade(Trade otherTrade)
        {
            return new Trade
            {
                StockShares = StockShares.Concat(otherTrade.StockShares).ToList(),
                Buyer = Buyer == otherTrade.Buyer ? Buyer : new Trader(),
                Seller = Seller == otherTrade.Seller ? Seller : new Trader(),
                TradePrice = TradePrice += otherTrade.TradePrice,
                Id = -1
            };
        }
    }
}
