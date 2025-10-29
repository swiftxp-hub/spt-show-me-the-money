using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class FleaPriceService(RagfairPriceService ragfairPriceService,
    RagfairOfferService ragfairOfferService,
    PaymentHelper paymentHelper)
{
    public ConcurrentDictionary<MongoId, double> Get()
    {
        Dictionary<MongoId, double> fleaPrices = ragfairPriceService.GetAllFleaPrices();
        ConcurrentDictionary<MongoId, double> result = new(fleaPrices);

        try
        {
            Parallel.ForEach(fleaPrices, (KeyValuePair<MongoId, double> fleaPrice) =>
            {
                IEnumerable<RagfairOffer>? offers = ragfairOfferService.GetOffersOfType(fleaPrice.Key);
                if (offers != null && offers.Any())
                {
                    double averageOffersPrice = GetAveragePriceFromOffers(offers);

                    if (averageOffersPrice > 0)
                        result[fleaPrice.Key] = averageOffersPrice;
                }
            });
        }
        catch (Exception)
        {
            return new ConcurrentDictionary<MongoId, double>(fleaPrices);
        }

        return result;
    }

    protected double GetAveragePriceFromOffers(IEnumerable<RagfairOffer> offers)
    {
        var sum = 0d;
        var totalOfferCount = 0;

        foreach (RagfairOffer offer in offers)
        {
            if (offer.Requirements!.Any(req => !paymentHelper.IsMoneyTpl(req.TemplateId))
                || offer.IsTraderOffer()
                || offer.IsPlayerOffer())
            {
                continue;
            }

            double offerItemCount = offer.SellInOnePiece.GetValueOrDefault(false) ? offer.Items!.First().Upd?.StackObjectsCount ?? 1 : 1;
            double? perItemPrice = offer.RequirementsCost / offerItemCount;

            sum += perItemPrice!.Value;
            totalOfferCount++;
        }

        if (sum > 0d && totalOfferCount > 0d)
            return Math.Round(sum / totalOfferCount);

        return 0d;
    }
}