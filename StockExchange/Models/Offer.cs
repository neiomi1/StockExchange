namespace StockExchange.Models
{
    public class Offer : BaseEntity
    {
        public Guid Id { get; set; }

        public decimal? PriceTotal { get; set; }

        public decimal PricePerShare { get; set; }

        public required ICollection<StockShare> StockShares { get; set; }

        public required Company Company { get; set; }
    }
}
