
using Microsoft.EntityFrameworkCore;
using StockExchange.Helpers;
using System.Diagnostics;

namespace StockExchange.Services
{
    public class SimulationService : BackgroundService
    {
        private StockMarketTimeProvider _stockMarketTimeProvider { get; init; }
        private readonly ILogger<SimulationService> _logger;

        public IServiceProvider Services { get; }

        public SimulationService(StockMarketTimeProvider stockMarketTimeProvider, IServiceProvider services, ILogger<SimulationService> logger)
        {
            _stockMarketTimeProvider = stockMarketTimeProvider;
            Services = services;
            _logger = logger;
        }

        private async Task SimulationLoop(CancellationToken cancellationToken)
        {
            using (var scope = Services.CreateScope())
            {
                await SetAppTime(scope);

                var previousTime = _stockMarketTimeProvider.GetUtcNow();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentTime = _stockMarketTimeProvider.GetUtcNow();
                    Console.WriteLine($"CurrentTime {currentTime}");
                    Debug.WriteLine($"CurrentTime {currentTime}");
                    await UpdateAppTime(scope);

                    // new day
                    if(previousTime.Day < currentTime.Day)
                    {
                        var rand = new Random();
                        if(rand.NextDouble() > 0.1)
                        {
                            using var db = scope.ServiceProvider.GetRequiredService<StockExchangeDb>();
                            await CreateNews(db);
                        }
                    }

                    await Task.Delay(1000);
                    previousTime = currentTime;
                }
            }
        }

        private async Task SetAppTime(IServiceScope scope)
        {
            if (_stockMarketTimeProvider.LastTickAppTime == DateTimeOffset.MinValue)
            {
                using var db = scope.ServiceProvider.GetRequiredService<StockExchangeDb>();
                var setting = await db.Settings.FirstOrDefaultAsync(setting => setting.Name == "SimulationDate");
                if (setting == null)
                {
                    setting = new Models.Setting { Name = "SimulationDate", Value = DateTimeOffset.MinValue.ToString() };
                    await db.Settings.AddAsync(setting);
                    await db.SaveChangesAsync();
                }
                if (DateTimeOffset.TryParse(setting.Value, out var date))
                {
                    _stockMarketTimeProvider.LastTickAppTime = date;
                }
            }
        }

        private async Task CreateNews(StockExchangeDb db)
        {
            var tags = await db.Tags.AsNoTracking().ToListAsync();
            var distinctTags = tags.Distinct();
            var news = RandomCompanyNews.GetRandomNews(tags);
            Debug.WriteLine($"Creating news {news.Title}");
            var newsTags = news.NewsTags;
            news.NewsTags = new List<Models.NewsTag>();
            await db.News.AddAsync(news);
            await db.SaveChangesAsync();
            news.NewsTags = newsTags;
            foreach (var tag in news.NewsTags)
            {
                tag.NewsId = news.Id;   
            }
            await db.SaveChangesAsync();
        }


        private async Task QuarterlyReport(StockExchangeDb db)
        {
            var companies = db.Companies.Include(c => c.Tags).ToList();

            companies.ForEach(c =>
            {
                var latestReport = db.Reports.Where(r => r.Company == c).OrderBy(r => r.CreatedDate).FirstOrDefault();
                //var news = db.News.Include(n => n.NewsTags).Where(n => n.)
            });
        }

        private async Task UpdateAppTime(IServiceScope scope)
        {
            using var db = scope.ServiceProvider.GetRequiredService<StockExchangeDb>();
            var setting = await db.Settings.FirstAsync(setting => setting.Name == "SimulationDate");
            setting.Value = _stockMarketTimeProvider.LastTickAppTime.ToString();
            await db.SaveChangesAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await SimulationLoop(cancellationToken);
        }
    }
}
