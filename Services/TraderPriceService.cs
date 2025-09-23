using System;
using EFT;
using EFT.InventoryLogic;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.Constants;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.Common.Sessions;
using SwiftXP.SPT.ShowMeTheMoney.Enums;
using SwiftXP.SPT.ShowMeTheMoney.Extensions;
using SwiftXP.SPT.ShowMeTheMoney.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Services;

public class TraderPriceService
{
    private static readonly Lazy<TraderPriceService> instance = new(() => new TraderPriceService());

    private TraderPriceService() { }

    public bool GetBestTraderPrice(TradeItem tradeItem)
    {
        TradePrice? highestTraderPrice = null;
        foreach (TraderClass trader in SptSession.Session.Traders)
        {
            if (this.IsTraderAvailable(trader))
            {
                TraderClass.GStruct264? singleObjectPrice = null;
                TraderClass.GStruct264? totalPrice = null;

                bool hasPrice = this.TryGetTraderUserItemPrice(trader, tradeItem, out singleObjectPrice, out totalPrice);
                if (hasPrice && (!Plugin.Configuration!.RoublesOnly.IsEnabled() || singleObjectPrice!.Value.CurrencyId.ToString() == SptConstants.CurrencyIds.Roubles))
                {
                    MongoID? currencyId = singleObjectPrice!.Value.CurrencyId;
                    double? currencyCourse = this.GetCurrencyCourse(trader, currencyId);
                    double itemPrice = singleObjectPrice.Value.Amount;

                    int? totalItemPrice = totalPrice != null ? totalPrice.Value.Amount : null;

                    TradePrice traderPrice = new(
                        tradeItem,
                        trader.Id,
                        trader.LocalizedName,
                        singleObjectPrice.Value.Amount,
                        totalItemPrice,
                        currencyCourse,
                        currencyId
                    );

                    if (highestTraderPrice == null || traderPrice.GetComparePriceInRouble() > highestTraderPrice.GetComparePriceInRouble())
                        highestTraderPrice = traderPrice;
                }
            }
        }

        tradeItem.TraderPrice = highestTraderPrice;

        return tradeItem.TraderPrice is not null;
    }

    private bool IsTraderAvailable(TraderClass trader)
    {
        return trader.Info.Available && !trader.Info.Disabled && trader.Info.Unlocked;
    }

    private bool TryGetTraderUserItemPrice(TraderClass trader, TradeItem tradeItem,
        out TraderClass.GStruct264? singleObjectPrice, out TraderClass.GStruct264? totalPrice)
    {
        singleObjectPrice = null;
        totalPrice = null;

        try
        {
            Item singleItem = tradeItem.Item.CloneItem();
            singleItem.StackObjectsCount = 1;
            singleObjectPrice = trader.GetUserItemPrice(singleItem);

            totalPrice = trader.GetUserItemPrice(tradeItem.Item);
        }
        catch (Exception)
        {
            SimpleSptLogger.Instance.LogDebug($"Could not get price from trader \"{trader.LocalizedName}\". Skipping.");
        }

        return singleObjectPrice is not null;
    }

    private double? GetCurrencyCourse(TraderClass trader, MongoID? currencyId)
    {
        if (!currencyId.HasValue)
            return null;

        double? result = null;

        switch (Plugin.Configuration!.CurrencyConversionMode.Value)
        {
            case CurrencyConversionEnum.Traders:
                if (currencyId.ToString() == SptConstants.CurrencyIds.Euros)
                    result = CurrencyPurchasePricesService.Instance.CurrencyPurchasePrices?.EUR;

                if (currencyId.ToString() == SptConstants.CurrencyIds.Dollars)
                    result = CurrencyPurchasePricesService.Instance.CurrencyPurchasePrices?.USD;

                break;
        }

        if (!result.HasValue)
            result = trader.GetSupplyData()?.CurrencyCourses[currencyId.Value];

        return result ?? 1;
    }

    public static TraderPriceService Instance => instance.Value;
}