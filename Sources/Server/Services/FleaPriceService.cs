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
        List<RagfairOffer> countableOffers = [.. offers
            .Where(x => !x.Requirements!.Any(req => !paymentHelper.IsMoneyTpl(req.TemplateId))
                && !x.IsTraderOffer()
                && !x.IsPlayerOffer())];

        if (countableOffers.Count > 0)
        {
            double offerSum = countableOffers.Sum(x =>
            {
                double itemCount = x.SellInOnePiece.GetValueOrDefault(false)
                    ? x.Items!.First().Upd?.StackObjectsCount
                    ?? 1 : 1;

                double? perItemPrice = x.RequirementsCost / itemCount;
                return perItemPrice ?? 0d;
            });

            if (offerSum > 0d)
                return Math.Round(offerSum / countableOffers.Count);
        }

        return 0d;
    }
}