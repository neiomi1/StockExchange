namespace StockExchange.Models
{
    public class StockShare : IBaseEntity
    {
        public int Id { get; set; }
        public required Company Company { get; set; }
        public Trader? Trader { get; set; }
        public Offer? Offer { get; set; }
        public decimal CompanyShare { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
