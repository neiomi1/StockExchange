using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StockExchange.Models;
using StockExchange.Models.ResponseModels;
using StockExchange.Services.UserService;
using System.Security.Claims;

namespace StockExchange.Services.OfferService
{
    public class OfferService : IOfferService
    {
        private readonly StockExchangeDb _stockExchangeDbContext;
        private readonly IUserService _userService;

        public OfferService(StockExchangeDb stockExchangeDbContext, IUserService userService)
        {
            _stockExchangeDbContext = stockExchangeDbContext;
            _userService = userService;
        }

        public async Task<IResult> CreateOffer(ClaimsPrincipal user, int companyId, int amount, decimal price, decimal? pricePerShare = null)
        {
            var trader = await _userService.GetTraderAsync(user);

            if (trader == null) { return TypedResults.NotFound(); }

            var ownedShares = _stockExchangeDbContext.StockShares.Where(share => share.Trader.Id == trader.Id && share.Company.Id == companyId).Take(amount).ToArray();

            if (ownedShares == null || !ownedShares.Any()) { return TypedResults.BadRequest(); }

            var offer = new Offer { StockShares = ownedShares, PriceTotal = price, PricePerShare = pricePerShare ?? price / amount, Company = ownedShares.First().Company };
            _stockExchangeDbContext.Offers.Add(offer);

            foreach (var share in ownedShares)
            {
                share.Offer = offer;
            }
            await _stockExchangeDbContext.SaveChangesAsync();

            return TypedResults.Created(offer.Id.ToString());
        }

        public async Task<IResult> BuyShares(ClaimsPrincipal claimsPrincipal, int companyId, int amount, decimal? maxPricePerShare = null)
        {
            var trader = await _userService.GetTraderAsync(claimsPrincipal);
            if (trader == null) { return TypedResults.NotFound(); }

            var buyResult = new TradeDto();
            while (amount > 0)
            {
                var currentResponse = await BuyCheapestOffer(trader, companyId, amount, maxPricePerShare);
                if (currentResponse is NotFound)
                {
                    return TypedResults.Ok(buyResult);
                }
                var currentTradeResult = (TradeDto)currentResponse;
                amount -= currentTradeResult.Amount;
                buyResult = buyResult.CombineTrade(currentTradeResult);
            }

            return TypedResults.Ok(buyResult);
        }

        public async Task<IResult> BuyCheapestOffer(Trader trader, int companyId, int amount, decimal? maxPricePerShare)
        {
            var offer = await _stockExchangeDbContext.Offers.
                   Include(o => o.Company)
                   .Include(o => o.StockShares)
                   .Where(offer => offer.Company.Id == companyId && maxPricePerShare <= offer.PricePerShare)
                   .OrderByDescending(o => o.PricePerShare)
                   .FirstAsync();

            if (offer == null) { return TypedResults.NotFound(); }

            var result = await BuyOffer(trader, offer.Id, amount);

            if (result is NotFound)
            {
                return await BuyCheapestOffer(trader, companyId, amount, maxPricePerShare);
            }
            return result;
        }


        public async Task<IResult> BuyOffer(ClaimsPrincipal claimsPrincipal, int offerId, int? amount = null)
        {
            var trader = await _userService.GetTraderAsync(claimsPrincipal);
            return await BuyOffer(trader, offerId, amount);
        }


        private async Task<IResult> BuyOffer(Trader? trader, int offerId, int? amount = null)
        {
            if (trader == null) { return TypedResults.NotFound(); }

            try
            {
                var offer = await _stockExchangeDbContext.Offers.FindAsync(offerId);
                if (offer == null) { return TypedResults.NotFound(); }

                var shares = await _stockExchangeDbContext.StockShares.Include(s => s.Offer).Where(share => share.Offer == offer).Take(amount ?? int.MaxValue).ToListAsync();
                var seller = shares.First().Trader;
                foreach (var share in shares)
                {
                    share.Trader = trader;
                }

                var trade = new Trade { Seller = seller, Buyer = trader, StockShares = shares, TradePrice = offer.PriceTotal != null ? offer.PriceTotal.Value : shares.Count * offer.PricePerShare };
                _stockExchangeDbContext.Trades.Add(trade);
                _stockExchangeDbContext.Offers.Remove(offer);

                await _stockExchangeDbContext.SaveChangesAsync();

                return TypedResults.Ok(new TradeDto(trade));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return await BuyOffer(trader, offerId, amount);
            }
        }

    }
}
