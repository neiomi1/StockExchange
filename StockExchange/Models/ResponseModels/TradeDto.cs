namespace StockExchange.Models.ResponseModels
{
    public class TradeDto
    {
        public int Amount { get; set; }
        public decimal TradePrice { get; set; }


        public TradeDto() { }
        public TradeDto(Trade trade)
        {
            Amount = trade.StockShares.Count;
            TradePrice = trade.TradePrice;
        }

        public TradeDto CombineTrade(TradeDto otherTrade)
        {
            return new TradeDto
            {
                Amount = Amount += otherTrade.Amount,
                TradePrice = TradePrice += otherTrade.TradePrice
            };
        }
    }
}
