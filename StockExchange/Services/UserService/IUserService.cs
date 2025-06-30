using StockExchange.Models;
using System.Security.Claims;

namespace StockExchange.Services.UserService
{
    public interface IUserService
    {
        Task<Trader?> GetTraderAsync(ClaimsPrincipal principal);
    }
}