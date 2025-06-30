namespace StockExchange.Models
{
    public class CompanyReport : BaseEntity
    {
        public int Id { get; set; }

        public Company Company { get; set; }

        public decimal Revenue { get; set; }

        public decimal Profit { get; set; }

    }
}
