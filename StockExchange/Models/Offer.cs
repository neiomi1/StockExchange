namespace StockExchange.Models
{
    public class Offer : IBaseEntity
    {
        public int Id { get; set; }

        public decimal? PriceTotal { get; set; }

        public decimal PricePerShare { get; set; }

        public required ICollection<StockShare> StockShares { get; set; }

        public required Company Company { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
