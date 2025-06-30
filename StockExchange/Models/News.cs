namespace StockExchange.Models
{
    public class News : IBaseEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<NewsTag> NewsTags { get; set; } = new();
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
