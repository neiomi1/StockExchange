namespace StockExchange.Models
{
    public class Company : IBaseEntity
    {
        public int Id { get; set; }

        public string? CompanyName { get; set; }

        public decimal Volatility { get; set; }

        public decimal GrowthFactor { get; set; }

        public decimal ProfitFactor { get; set; }

        public required List<CompanyTag> Tags { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
