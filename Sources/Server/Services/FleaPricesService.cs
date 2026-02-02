using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader - 1)]
public class FleaPricesService(ISptLogger<FleaPricesService> sptLogger,
    ItemHelper itemHelper,
    RagfairPriceService ragfairPriceService,
    RagfairOfferService ragfairOfferService,
    PaymentHelper paymentHelper)
{
#pragma warning disable CA1859 // Use concrete types when possible for improved performance

    private IReadOnlyDictionary<string, double> _cachedPrices = new Dictionary<string, double>();
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    private DateTime _nextUpdate = DateTime.MinValue;

    private readonly Lock _lock = new();

    private const int CacheDurationSeconds = 60;

    public IReadOnlyDictionary<string, double> Get()
    {
        if (_nextUpdate > DateTime.UtcNow && _cachedPrices.Count > 0)
            return _cachedPrices;

        lock (_lock)
        {
            if (_nextUpdate > DateTime.UtcNow && _cachedPrices.Count > 0)
                return _cachedPrices;

            Stopwatch stopwatch = Stopwatch.StartNew();

            Dictionary<MongoId, double> fleaPrices = ragfairPriceService.GetAllFleaPrices();
            ConcurrentDictionary<string, double> tempResult = new(Environment.ProcessorCount, fleaPrices.Count);

            Parallel.ForEach(fleaPrices, fleaPrice =>
            {
                try
                {
                    double price;

                    MongoId itemId = fleaPrice.Key;
                    double fallbackPrice = fleaPrice.Value;

                    if (itemHelper.IsOfBaseclass(itemId, BaseClasses.WEAPON))
                        price = ragfairPriceService.GetStaticPriceForItem(itemId) ?? 0;
                    else
                        price = GetAveragePriceFromOffers(itemId);

                    if (IsValidPrice(price))
                        tempResult.TryAdd(itemId, price);

                    else if (IsValidPrice(fallbackPrice))
                        tempResult.TryAdd(itemId, fallbackPrice);
                }
                catch (Exception ex)
                {
                    sptLogger.Debug($"{Constants.LoggerPrefix}Error calculating price for item {fleaPrice.Key}: {ex}");

                    if (IsValidPrice(fleaPrice.Value))
                        tempResult.TryAdd(fleaPrice.Key, fleaPrice.Value);
                }
            });

            _cachedPrices = new Dictionary<string, double>(tempResult);
            _nextUpdate = DateTime.UtcNow.AddSeconds(CacheDurationSeconds);

            stopwatch.Stop();
            sptLogger.Debug($"{Constants.LoggerPrefix}Flea prices updated. Found {_cachedPrices.Count} items in {stopwatch.ElapsedMilliseconds}ms.");

            return _cachedPrices;
        }
    }

    private double GetAveragePriceFromOffers(MongoId itemTemplateId)
    {
        IEnumerable<RagfairOffer>? offers = ragfairOfferService.GetOffersOfType(itemTemplateId);

        if (offers == null)
            return 0d;

        double offerSum = 0;
        int countedOffers = 0;

        foreach (RagfairOffer offer in offers)
        {
            if (offer.IsTraderOffer() || offer.IsPlayerOffer())
                continue;

            if (offer.Items == null || offer.Items.Count == 0)
                continue;

            bool isMoneyOnly = true;
            if (offer.Requirements != null)
            {
                foreach (OfferRequirement requirement in offer.Requirements)
                {
                    if (!paymentHelper.IsMoneyTpl(requirement.TemplateId))
                    {
                        isMoneyOnly = false;
                        break;
                    }
                }
            }

            if (!isMoneyOnly)
                continue;

            Item firstItem = offer.Items[0];

            double itemCount = offer.SellInOnePiece.GetValueOrDefault(false)
                ? firstItem.Upd?.StackObjectsCount ?? 1
                : 1;

            if (itemCount <= 0)
                continue;

            double? perItemPrice = offer.RequirementsCost / itemCount;

            if (perItemPrice.HasValue && perItemPrice > 0)
            {
                offerSum += perItemPrice.Value;
                ++countedOffers;
            }
        }

        if (countedOffers > 0)
            return Math.Round(offerSum / countedOffers);

        return 0d;
    }

    private static bool IsValidPrice(double price)
    {
        return price > 0d && !double.IsNaN(price) && !double.IsInfinity(price);
    }
}