using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockExchange;
using StockExchange.Models;
using StockExchange.Services;
using StockExchange.Services.OfferService;
using StockExchange.Services.UserService;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<StockExchangeDb>(opt =>
opt.UseNpgsql(builder.Configuration.GetConnectionString("StockExchange"))
.UseSnakeCaseNamingConvention()
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAuthentication().AddJwtBearer().AddJwtBearer("LocalAuthIssuer");
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<Trader>().AddEntityFrameworkStores<StockExchangeDb>();

builder.Services.Configure<StockExchangeTimeOptions>(builder.Configuration.GetSection(nameof(StockExchangeTimeOptions)));

//Swagger/Openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<SimulationService>();

builder.Services.AddSingleton<StockMarketTimeProvider>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IOfferService, OfferService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var accountGroup = app.MapGroup("/account");
accountGroup.MapIdentityApi<Trader>();


await using var scope = app.Services.CreateAsyncScope();
var _stockExchangeDbContext = scope.ServiceProvider.GetRequiredService<StockExchangeDb>();
var canConnect = await _stockExchangeDbContext.Database.CanConnectAsync();
app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);

var db = scope.ServiceProvider.GetRequiredService<StockExchangeDb>();
db.Database.Migrate(); // Applies migrations
SeedData.SeedTags(db, "SeedData/Tags.json"); // Path to your seed file
SeedData.SeedCompanies(db, "SeedData/Companies.json"); // Path to your seed file

#region Settings
var settings = app.MapGroup("/settings");
settings.MapPatch("/", UpdateTimeOptions).RequireAuthorization();
static void UpdateTimeOptions(StockMarketTimeProvider timer, StockExchangeTimeOptions options)
{
    timer.ProviderOptions = options;
}


#endregion Settings

#region Exchange 

var exchange = app.MapGroup("/offer");
exchange.MapPost("/", Offer).RequireAuthorization();
exchange.MapGet("/", GetOffers).RequireAuthorization();
exchange.MapGet("/all", GetAllOffers).RequireAuthorization();
exchange.MapPost("/buy", BuyOffer).RequireAuthorization();

static async Task<IResult> Offer(ClaimsPrincipal user, IOfferService offerService, Guid companyGuid, int amount, decimal price, decimal? pricePerShare = null)
{
    return await offerService.CreateOffer(user, companyGuid, amount, price, pricePerShare);
}

static async Task<IResult> GetOffers(ClaimsPrincipal user, StockExchangeDb db, UserManager<Trader> userManager)
{
    var trader = await userManager.GetUserAsync(user);

    if (trader == null) { return TypedResults.NotFound(); }

    return TypedResults.Ok(await db.StockShares.Where(share => share.Trader.Id == trader.Id).Include(o => o.Offer).Select(o => o.Offer).Distinct().ToListAsync());
}

static async Task<IResult> GetAllOffers(ClaimsPrincipal user, StockExchangeDb db, UserManager<Trader> userManager)
{
    return TypedResults.Ok(await db.Offers.ToArrayAsync());
}

static async Task<IResult> BuyOffer(ClaimsPrincipal user, IOfferService offerService, Guid offerId, int? amount = null)
{
   return await offerService.BuyOffer(user, offerId, amount);
}

#endregion Exchange

#region Company

var company = app.MapGroup("/company");
company.MapGet("/", GetAllCompanies).RequireAuthorization();
company.MapGet("/{id}", GetCompany);
company.MapPost("/", CreateCompany);
company.MapPut("/{id}", UpdateCompany);

static async Task<IResult> GetAllCompanies(StockExchangeDb db)
{
    return TypedResults.Ok(await db.Companies.ToArrayAsync());
}

static async Task<IResult> GetCompany(StockExchangeDb db, Guid id)
{
    return await db.Companies.FindAsync(id)
    is Company company ?
    TypedResults.Ok(company)
    : TypedResults.NotFound();
}

static async Task<IResult> UpdateCompany(StockExchangeDb db, Guid id, Company company)
{
    var dbCompany = await db.Companies.FindAsync(id);

    if (dbCompany is null) return TypedResults.NotFound();

    dbCompany.CompanyName = company.CompanyName;
    dbCompany.Tags = company.Tags;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> CreateCompany(StockExchangeDb db, Company company)
{
    await db.AddAsync(company);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/traders/{company.Id}", company);
}

#endregion Company


#region Trader

var traders = app.MapGroup("/trader");
traders.MapGet("/", GetAllTraders).RequireAuthorization();
traders.MapGet("/{id}", GetTrader);
traders.MapPost("/", CreateTrader);
traders.MapPut("/{id}", UpdateTrader);

static async Task<IResult> GetAllTraders(StockExchangeDb db)
{
    return TypedResults.Ok(await db.Traders.ToArrayAsync());
}

static async Task<IResult> GetTrader(Guid id, StockExchangeDb db)
{
    return await db.Traders.FindAsync(id)
        is Trader trader ?
        TypedResults.Ok(trader)
        : TypedResults.NotFound();
}

static async Task<IResult> CreateTrader(Trader trader, StockExchangeDb db)
{
    db.Traders.Add(trader);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/traders/{trader.Id}", trader);
}

static async Task<IResult> UpdateTrader(Guid id, Trader traderUpdate, StockExchangeDb db)
{
    var trader = await db.Traders.FindAsync(id);

    if (trader is null) return TypedResults.NotFound();

    trader.UserName = traderUpdate.UserName;
    trader.Cash = traderUpdate.Cash;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}


#endregion Trader

#region StockShare

var stockShare = app.MapGroup("/shares");
stockShare.MapGet("/", GetShares).RequireAuthorization();
stockShare.MapGet("/{id}", GetTrader);
stockShare.MapPost("/", CreateTrader);
stockShare.MapPut("/{id}", UpdateTrader);
company.MapPost("/buy", BuyShares).RequireAuthorization();

static async Task<IResult> GetShares(UserManager<Trader> userManager, ClaimsPrincipal user, StockExchangeDb db)
{
    var trader = await userManager.GetUserAsync(user);
    if (trader == null) { return TypedResults.NotFound(); }

    return TypedResults.Ok(await db.StockShares.Where(share => share.Trader.Id == trader.Id).ToArrayAsync());
}

static async Task<IResult> GetShare(Guid id, StockExchangeDb db)
{
    return await db.Traders.FindAsync(id)
        is Trader trader ?
        TypedResults.Ok(trader)
        : TypedResults.NotFound();
}

//static async Task<IResult> CreateShares(StockExchangeDb db, Guid companyGuid, int amount)
//{
//    var company = await db.Companies.FindAsync(companyGuid);
//    if (company == null) { return TypedResults.NotFound(); }

//    var share = new StockShare { Company =  company, }
//    db.StockShares.AddRangeAsync()
//    db.StockShares.Add(trader);
//    await db.SaveChangesAsync();

//    return TypedResults.Created($"/traders/{trader.Id}", trader);
//}

//static async Task<IResult> UpdateTrader(Guid id, Trader traderUpdate, StockExchangeDb db)
//{
//    var trader = await db.Traders.FindAsync(id);

//    if (trader is null) return TypedResults.NotFound();

//    trader.UserName = traderUpdate.UserName;
//    trader.Cash = traderUpdate.Cash;
//    await db.SaveChangesAsync();
//    return TypedResults.NoContent();
//}


static async Task<IResult> BuyShares(ClaimsPrincipal user, IOfferService offerService, Guid companyGuid, int amount, decimal? maxPricePerShare = null)
{
    return await offerService.BuyShares(user, companyGuid, amount, maxPricePerShare);
}


#endregion StockShare

//app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
//{
//    if (await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.NoContent();
//    }

//    return Results.NotFound();
//});

await app.RunAsync();