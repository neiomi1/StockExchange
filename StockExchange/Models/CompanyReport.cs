namespace StockExchange.Models
{
    public class CompanyReport : IBaseEntity
    {
        public int Id { get; set; }

        public Company Company { get; set; }

        public decimal Revenue { get; set; }

        public decimal Profit { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
