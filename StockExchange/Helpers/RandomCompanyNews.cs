using StockExchange.Models;

namespace StockExchange.Helpers
{
    public static class RandomCompanyNews
    {


        private static readonly List<string> Actions = new()
{
    "announces", "reports", "reveals", "faces", "confirms", "denies", "finalizes", "experiences",
    "launches", "completes", "delays", "accelerates", "negotiates", "secures", "approves", "rejects",
    "suspends", "investigates", "expands", "partners with", "invests in", "acquires", "files for",
    "settles", "warns", "surpasses", "misses", "restructures", "cuts", "adds", "introduces",
    "integrates", "licenses", "wins", "loses", "transfers", "updates"
};

        private static readonly List<string> Objects = new()
{
    "record profits", "massive data breach", "executive shakeup", "product launch", "layoffs",
    "merger deal", "restructuring plan", "stock surge", "market withdrawal", "regulatory approval",
    "cyberattack", "new R&D initiative", "IPO filing", "funding round", "AI integration",
    "cloud migration", "customer data leak", "sustainability program", "antitrust probe",
    "leadership change", "factory expansion", "revenue miss", "compliance audit",
    "capital investment", "patent dispute", "class action lawsuit", "new CEO appointment",
    "service outage", "hackathon win", "international expansion", "product recall",
    "cost-cutting measures", "IPO pricing", "token launch", "NFT marketplace", "AI-powered upgrade"
};

        private static readonly List<string> Contexts = new()
{
    "amid economic uncertainty", "after strong Q2 earnings", "despite supply chain issues",
    "ahead of IPO", "in response to new legislation", "following CEO resignation",
    "to enter new market", "amid investor pressure", "as part of expansion strategy",
    "under regulatory review", "after internal investigation", "during annual conference",
    "in light of cyber threats", "after customer backlash", "amid rising inflation",
    "in post-pandemic recovery", "after securing funding", "during board reshuffle",
    "ahead of earnings call", "to meet ESG goals", "in preparation for merger",
    "following strategic review", "after rival acquisition", "amid layoffs in tech sector",
    "under global scrutiny", "in response to shareholder demands", "as part of digital transformation",
    "following IPO success", "amid AI ethics debate", "to address chip shortage",
    "after whistleblower revelations", "amid talent shortage", "during legal proceedings"
};


        private static string GenerateTitle(Random random, List<NewsTag> tags)
        {
            var mainTag = tags.OrderBy(t => t.Weight).First();
            var action = Actions.SelectionSample(1, random).First();
            var obj = Objects.SelectionSample(1, random).First();
            var context = Contexts.SelectionSample(1, random).First();

            return $"{mainTag.Tag.Name} {action} {obj} {context}.";
        }

        public static News GetRandomNews(ICollection<Tag> tags)
        {
            var random = new Random();
            var severity = Math.Round(random.NextDouble() * random.Next(10), 2);
            var selectedTags = tags.ToList().SelectionSample(random.Next(1, 5)).Distinct().Select(t => new NewsTag { Tag = t, Weight = (random.NextDouble() - 0.5) * severity}).ToList();
            
            return new News
            {
                Title = GenerateTitle(random, selectedTags),
                NewsTags = selectedTags,
            };
        }
    }
}
