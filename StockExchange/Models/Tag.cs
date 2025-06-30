namespace StockExchange.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<NewsTag> NewsTags { get; set; } = new();
        public List<CompanyTag> CompanyTags { get; set; } = new();
    }
}
