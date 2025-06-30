using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockExchange.Models;
using StockExchange.Services;

namespace StockExchange
{
    public class StockExchangeDb : IdentityDbContext<Trader>
    {
        private readonly StockMarketTimeProvider _timeProvider;
        public StockExchangeDb(DbContextOptions<StockExchangeDb> options, StockMarketTimeProvider timeProvider)
        : base(options) {
            _timeProvider = timeProvider;
        }

        public DbSet<StockShare> StockShares => Set<StockShare>();
        public DbSet<Trade> Trades => Set<Trade>();
        public DbSet<Trader> Traders => Set<Trader>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<CompanyReport> Reports => Set<CompanyReport>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<News> News => Set<News>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<NewsTag> WeightedTags => Set<NewsTag>();
        public DbSet<Setting> Settings => Set<Setting>();

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = _timeProvider.GetUtcNow();

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = _timeProvider.GetUtcNow();
                }
            }


            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
              .Entries()
              .Where(e => e.Entity is BaseEntity && (
              e.State == EntityState.Added
              || e.State == EntityState.Modified));

            var date = _timeProvider.GetUtcNow();

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = date;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = date;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
