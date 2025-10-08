using System;
using System.Linq;
using EFT.InventoryLogic;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Services;

public class FleaPriceService
{
    private static readonly Lazy<FleaPriceService> instance = new(() => new FleaPriceService());

    private FleaPriceService() { }

    public bool GetFleaPrice(TradeItem tradeItem, bool includeTaxInPrices)
    {
        if (RagfairPriceTableService.Instance.Prices is not null
            && RagfairPriceTableService.Instance.Prices!.TryGetValue(tradeItem.Item.TemplateId, out double fleaPrice))
        {
            SetFleaPriceOfTradeItem(tradeItem, fleaPrice, includeTaxInPrices);
        }

        return tradeItem.FleaPrice is not null;
    }

    private static void SetFleaPriceOfTradeItem(TradeItem tradeItem, double fleaPrice, bool includeTaxInPrices)
    {
        int fleaPriceAfterMultiply = (int)Math.Round(fleaPrice * (double)Plugin.Configuration!.FleaPriceMultiplicand.GetValue());

        double? singleObjectTaxPrice = null;
        double? totalTaxPrice = null;

        if (Plugin.Configuration!.IncludeFleaTax || Plugin.Configuration!.ShowFleaTax)
        {
            singleObjectTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, 1, fleaPriceAfterMultiply, false);
            totalTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, tradeItem.ItemObjectCount, fleaPriceAfterMultiply, false);
        }

        TradePrice tradePrice =
            new(
                tradeItem,

                null,
                GetFleaMarketName(),

                fleaPriceAfterMultiply,
                null,

                null,
                null,

                singleObjectTaxPrice,
                totalTaxPrice,

                includeTaxInPrices
            );

        tradeItem.FleaPrice = tradePrice;
    }

    private static string GetFleaMarketName()
    {
        try
        {
            string fleaMarketName = "RAG FAIR".Localized(null);
            return fleaMarketName.First().ToString().ToUpperInvariant()
                + fleaMarketName.ToLowerInvariant().Substring(1);
        }
        catch (Exception) { }

        return "Flea";
    }

    public static FleaPriceService Instance => instance.Value;
}