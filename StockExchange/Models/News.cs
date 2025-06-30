namespace StockExchange.Models
{
    public class News : BaseEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<NewsTag> NewsTags { get; set; } = new();
    }
}
