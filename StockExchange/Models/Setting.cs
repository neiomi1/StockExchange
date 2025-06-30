using Microsoft.EntityFrameworkCore;

namespace StockExchange.Models
{
    public class Setting
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public string? Value { get; set; }
    }
}
