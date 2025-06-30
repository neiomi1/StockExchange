namespace StockExchange.Models
{
    public class StockShare : BaseEntity
    {
        public Guid Id {  get; set; }
        public required Company Company { get; set; }
        public Trader? Trader { get; set; }
        public Offer? Offer { get; set; }

        public decimal CompanyShare {  get; set; }
    }
}
