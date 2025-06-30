using StockExchange.Models;
using System.Security.Claims;

namespace StockExchange.Services.OfferService
{
    public interface IOfferService
    {
        Task<IResult> BuyCheapestOffer(Trader trader, Guid companyGuid, int amount, decimal? maxPricePerShare);
        Task<IResult> BuyOffer(ClaimsPrincipal claimsPrincipal, Guid offerId, int? amount = null);
        Task<IResult> BuyShares(ClaimsPrincipal claimsPrincipal, Guid companyGuid, int amount, decimal? maxPricePerShare = null);
        Task<IResult> CreateOffer(ClaimsPrincipal user, Guid companyGuid, int amount, decimal price, decimal? pricePerShare = null);
    }
}