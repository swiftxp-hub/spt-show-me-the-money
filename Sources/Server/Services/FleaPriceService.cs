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
public class FleaPriceService(ISptLogger<ShowMeTheMoneyStaticRouter> sptLogger,
    ItemHelper itemHelper,
    RagfairPriceService ragfairPriceService,
    RagfairOfferService ragfairOfferService,
    PaymentHelper paymentHelper)
{
    public ConcurrentDictionary<MongoId, double> Get()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Dictionary<MongoId, double> fleaPrices = ragfairPriceService.GetAllFleaPrices();
        ConcurrentDictionary<MongoId, double> result = new(fleaPrices);

        Parallel.ForEach(result, fleaPrice =>
        {
            if (!itemHelper.ArmorItemHasRemovablePlateSlots(fleaPrice.Key))
            {
                double newPrice = GetAveragePriceFromOffers(fleaPrice.Key);
                if (newPrice > 0d)
                    result[fleaPrice.Key] = newPrice;
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
                List<RagfairOffer> countableOffers = [.. offers
                    .Where(x => !x.Requirements!.Any(req => !paymentHelper.IsMoneyTpl(req.TemplateId))
                        && !x.IsTraderOffer()
                        && !x.IsPlayerOffer())];

                if (countableOffers.Count > 0)
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
        catch (Exception) { }

        return 0d;
    }
}