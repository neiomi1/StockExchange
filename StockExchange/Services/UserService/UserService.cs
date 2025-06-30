
using Microsoft.AspNetCore.Identity;
using StockExchange.Models;
using System.Security.Claims;

namespace StockExchange.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<Trader> _userManager;

        public UserService(UserManager<Trader> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Trader?> GetTraderAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }
    }
}
