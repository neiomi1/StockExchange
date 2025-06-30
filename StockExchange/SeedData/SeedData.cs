using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using StockExchange;
using StockExchange.Models;

public static class SeedData
{
    public static void SeedTags(StockExchangeDb context, string filePath)
    {
        var existingTags = context.Tags.ToList();

        var json = File.ReadAllText(filePath);
        var tags = JsonConvert.DeserializeObject<List<Tag>>(json);
       
        if (tags != null)
        {
            context.Tags.AddRange(tags.Where(tag => !existingTags.Any(t => t.Name == tag.Name)));
            context.SaveChanges();
        }
    }
    public static void SeedCompanies(StockExchangeDb context, string filePath)
    {
        var existingCompanies = context.Companies.ToList();

        var json = File.ReadAllText(filePath);
        var jsonCompanies = JsonConvert.DeserializeObject<List<SeedCompany>>(json);
        var companies = jsonCompanies?.Select(t => t.ToCompany());

        if (companies != null)
        {
            companies = companies.Where(tag => !existingCompanies.Any(t => t.CompanyName == tag.CompanyName)).ToList();
            context.Companies.AddRange(companies);
            context.SaveChanges(); 
            var tags = context.Tags.ToList();
            foreach (var company in companies)
            {
                company.Tags = 
                    jsonCompanies.Find(c => c.CompanyName == company.CompanyName)?
                    .Tags.
                    Where(tag => tags.Any(t => t.Name == tag))
                    .Select(tag => new CompanyTag { Company = company, CompanyId =company.Id, Tag = tags.FirstOrDefault(t => t.Name == tag), TagId = tags.FirstOrDefault(t => t.Name == tag).Id  }).ToList();
            }
            context.SaveChanges();
        }
        
    }

    private class SeedCompany
    {
        public Guid Id { get; set; }

        public string? CompanyName { get; set; }

        public decimal Volatility { get; set; }

        public decimal GrowthFactor { get; set; }

        public decimal ProfitFactor { get; set; }

        public required List<string> Tags { get; set; }

        public Company ToCompany()
        {
           
            return new Company { CompanyName = CompanyName, Volatility = Volatility, GrowthFactor = GrowthFactor, ProfitFactor = ProfitFactor, Tags = new List<CompanyTag>()};
        }
    }
}