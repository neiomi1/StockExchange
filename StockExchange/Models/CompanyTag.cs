using Microsoft.EntityFrameworkCore;

namespace StockExchange.Models
{
    [PrimaryKey(nameof(CompanyId), nameof(TagId))]
    public class CompanyTag
    {
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public double Weight { get; set; }
    }
}
