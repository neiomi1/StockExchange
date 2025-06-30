using Microsoft.Extensions.Options;
using StockExchange.Models;

namespace StockExchange.Services
{
    public class StockMarketTimeProvider : TimeProvider
    {
        
        public StockExchangeTimeOptions ProviderOptions { get; set => field = updateOptions(value); } 

        private DateTimeOffset _lastTickActualTime = DateTime.UtcNow;
        public DateTimeOffset LastTickAppTime { get; set; }

        private Lock _lock = new();

        public StockMarketTimeProvider(IOptions<StockExchangeTimeOptions> options)
        {
            ProviderOptions = options.Value;
        }
        
        private StockExchangeTimeOptions updateOptions(StockExchangeTimeOptions options)
        {
            LastTickAppTime = LastTickAppTime == DateTimeOffset.MinValue ? options?.StartTime ?? DateTimeOffset.MinValue : LastTickAppTime;
            return options;
        }

        // This works but needs to work atomically
        public override DateTimeOffset GetUtcNow()
        {
            _lock.Enter();
            var delta = base.GetUtcNow() - _lastTickActualTime;
            _lastTickActualTime = base.GetUtcNow();
            LastTickAppTime = LastTickAppTime.Add(delta.Multiply(ProviderOptions?.SimulationSpeed ?? 1.0));
            _lock.Exit();
            return LastTickAppTime;
        }

    }
}
