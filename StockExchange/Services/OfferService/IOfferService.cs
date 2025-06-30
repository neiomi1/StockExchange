using StockExchange.Models;
using System.Security.Claims;

namespace StockExchange.Services.OfferService
{
    public interface IOfferService
    {
        Task<IResult> BuyCheapestOffer(Trader trader, int companyId, int amount, decimal? maxPricePerShare);
        Task<IResult> BuyOffer(ClaimsPrincipal claimsPrincipal, int offerId, int? amount = null);
        Task<IResult> BuyShares(ClaimsPrincipal claimsPrincipal, int companyId, int amount, decimal? maxPricePerShare = null);
        Task<IResult> CreateOffer(ClaimsPrincipal user, int companyId, int amount, decimal price, decimal? pricePerShare = null);
    }
}