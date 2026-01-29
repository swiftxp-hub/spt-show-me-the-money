using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class FleaPricesService(ISptLogger<FleaPricesService> sptLogger,
    ItemHelper itemHelper,
    RagfairPriceService ragfairPriceService,
    RagfairOfferService ragfairOfferService,
    PaymentHelper paymentHelper)
{
    public ConcurrentDictionary<string, double> Get()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Dictionary<MongoId, double> fleaPrices = ragfairPriceService.GetAllFleaPrices();
        ConcurrentDictionary<string, double> result = new();

        Parallel.ForEach(fleaPrices, fleaPrice =>
        {
            try
            {
                double price;

                if (itemHelper.IsOfBaseclass(fleaPrice.Key, BaseClasses.WEAPON))
                    price = ragfairPriceService.GetStaticPriceForItem(fleaPrice.Key) ?? 0;
                else
                    price = GetAveragePriceFromOffers(fleaPrice.Key);

                if (price > 0d && !double.IsNaN(price) && !double.IsInfinity(price))
                    result.TryAdd(fleaPrice.Key, price);

                // Fallback to price which was delivered by RagfairPriceService.
                else if (fleaPrice.Value > 0d && !double.IsNaN(fleaPrice.Value) && !double.IsInfinity(fleaPrice.Value))
                    result.TryAdd(fleaPrice.Key, fleaPrice.Value);
            }
            catch (Exception ex)
            {
                sptLogger.Debug($"Error calculating price for item {fleaPrice.Key}: {ex}");

                // Fallback to price which was delivered by RagfairPriceService.
                if (fleaPrice.Value > 0d && !double.IsNaN(fleaPrice.Value) && !double.IsInfinity(fleaPrice.Value))
                    result.TryAdd(fleaPrice.Key, fleaPrice.Value);
            }
        });

        stopwatch.Stop();
        sptLogger.Debug($"FleaPriceService.Get() was finished in {stopwatch.ElapsedMilliseconds}ms.");

        return result;
    }

    private double GetAveragePriceFromOffers(MongoId itemTemplateId)
    {
        try
        {
            IEnumerable<RagfairOffer>? offers = ragfairOfferService.GetOffersOfType(itemTemplateId);
            if (offers != null && offers.Any())
            {
                IEnumerable<RagfairOffer> countableOffers = offers
                    .Where(x => !x.Requirements!.Any(req => !paymentHelper.IsMoneyTpl(req.TemplateId))
                        && !x.IsTraderOffer()
                        && !x.IsPlayerOffer());

                if (countableOffers.Any())
                {
                    double offerSum = 0;
                    int countedOffers = 0;

                    foreach (RagfairOffer ragfairOffer in countableOffers)
                    {
                        Item firstItem = ragfairOffer.Items!.First();
                        double itemCount = ragfairOffer.SellInOnePiece.GetValueOrDefault(false)
                            ? firstItem.Upd?.StackObjectsCount
                            ?? 1 : 1;

                        double? perItemPrice = ragfairOffer.RequirementsCost / itemCount;
                        if (perItemPrice.HasValue && perItemPrice > 0)
                        {
                            offerSum += perItemPrice.Value;
                            ++countedOffers;
                        }
                    }

                    if (offerSum > 0d)
                        return Math.Round(offerSum / countedOffers);
                }
            }
        }
        catch (Exception ex)
        {
            sptLogger.Debug($"Error getting average price for item {itemTemplateId}: {ex}");
        }

        return 0d;
    }
}