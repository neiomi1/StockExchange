using Microsoft.EntityFrameworkCore;

namespace StockExchange.Models
{
    [PrimaryKey(nameof(NewsId), nameof(TagId))]
    public class NewsTag
    {
        public int NewsId { get; set; }
        public News News { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public double Weight { get; set; }
    }
}
