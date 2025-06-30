namespace StockExchange.Models
{
    public sealed class StockExchangeTimeOptions
    {
        public double? SimulationSpeed { get; set; } = 1.0;
        public DateTime? StartTime { get; set; }
    }
}
