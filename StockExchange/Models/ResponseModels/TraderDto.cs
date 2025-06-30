namespace StockExchange.Models.ResponseModels
{
    public class TraderDto
    {
        public decimal Cash { get; set; }

        public string? Name { get; set; }

        public TraderDto()
        {
        }

        public TraderDto(Trader trader)
        {
            Cash = trader.Cash;
            Name = trader.UserName;
        }
    }
}
