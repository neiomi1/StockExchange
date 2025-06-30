namespace StockExchange.Models
{
    public class Company : BaseEntity
    {
        public Guid Id { get; set; }

        public string? CompanyName { get; set; }

        public decimal Volatility { get; set; }

        public decimal GrowthFactor { get; set; }

        public decimal ProfitFactor { get; set; }

        public required List<CompanyTag> Tags { get; set; }
    }
}
