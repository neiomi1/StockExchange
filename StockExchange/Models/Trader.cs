using Microsoft.AspNetCore.Identity;

namespace StockExchange.Models
{
    public class Trader : IdentityUser
    {
        public decimal Cash { get; set; }

        public Trader() { }
    }

}
