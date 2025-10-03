using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SwiftXP.SPT.ShowMeTheMoney.Server.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Server;

[Injectable]
public class ShowMeTheMoneyStaticRouter : StaticRouter
{
    private static HttpResponseUtil? HttpResponseUtil;

    private static DatabaseService? DatabaseService;

    private static RagfairOfferService? RagfairOfferService;

    public ShowMeTheMoneyStaticRouter(JsonUtil jsonUtil, HttpResponseUtil httpResponseUtil, DatabaseService databaseService, RagfairOfferService ragfairOfferService)
    : base(jsonUtil, GetCustomRoutes())
    {
        HttpResponseUtil = httpResponseUtil;
        DatabaseService = databaseService;
        RagfairOfferService = ragfairOfferService;
    }

    private static List<RouteAction> GetCustomRoutes()
    {
        return
        [
            new RouteAction(
                "/showMeTheMoney/getCurrencyPurchasePrices",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetCurrencyPurchasePricesAsync()
            ),

            new RouteAction(
                "/showMeTheMoney/getStaticPriceTable",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetStaticPriceTableAsync()
            ),

            new RouteAction(
                "/showMeTheMoney/getDynamicPriceTable",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetDynamicPriceTableAsync()
            )
        ];
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async ValueTask<CurrencyPurchasePrices> GetCurrencyPurchasePricesAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        Trader? peacekeeper = DatabaseService!.GetTrader("5935c25fb3acc3127c3d8cd9");
        Trader? skier = DatabaseService!.GetTrader("58330581ace78e27b8b10cee");

        double? eurPrice = skier?.Assort.BarterScheme["677536ee7949f87882036fb0"][0][0].Count;
        double? usdPrice = peacekeeper?.Assort.BarterScheme["676d24a5798491c5260f4b01"][0][0].Count;

        CurrencyPurchasePrices currencyPurchasePrices = new(eurPrice, usdPrice);

        return currencyPurchasePrices;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async ValueTask<Dictionary<MongoId, double>> GetStaticPriceTableAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        HandbookBase handbookTable = DatabaseService!.GetTables().Templates.Handbook;
        Dictionary<MongoId, TemplateItem> itemTable = DatabaseService!.GetTables().Templates.Items;
        Dictionary<MongoId, double> priceTable = DatabaseService!.GetTables().Templates.Prices;

        Dictionary<MongoId, double> clonedPriceTable = [];
        foreach (KeyValuePair<MongoId, TemplateItem> item in itemTable)
        {
            if (item.Value.Properties?.CanSellOnRagfair == true)
            {
                double? itemPrice = null;
                if (priceTable.TryGetValue(item.Key, out double price))
                {
                    itemPrice = price;
                }
                else
                {
                    itemPrice = handbookTable.Items.FirstOrDefault(x => x.Id == item.Key)?.Price;
                }

                if (itemPrice.HasValue)
                    clonedPriceTable.Add(item.Key, itemPrice.Value);
            }
        }

        return clonedPriceTable;
    }

    private static async ValueTask<Dictionary<MongoId, double>> GetDynamicPriceTableAsync()
    {
        Dictionary<MongoId, double> priceTable = await GetStaticPriceTableAsync();

        foreach (KeyValuePair<MongoId, double> price in priceTable)
        {
            double averageOffersPrice = 0;
            int countedOffers = 0;

            List<RagfairOffer>? ragfairOffersForType = RagfairOfferService!.GetOffersOfType(price.Key)?.ToList();
            if (ragfairOffersForType != null)
            {
                foreach (RagfairOffer offer in ragfairOffersForType)
                {
                    if (offer.User?.MemberType != MemberCategory.Trader && offer.RequirementsCost.HasValue)
                    {
                        averageOffersPrice += offer.RequirementsCost.Value;
                        ++countedOffers;
                    }
                }
            }

            if (averageOffersPrice > 0d)
            {
                priceTable[price.Key] = averageOffersPrice / countedOffers;
            }
        }

        return priceTable;
    }
}