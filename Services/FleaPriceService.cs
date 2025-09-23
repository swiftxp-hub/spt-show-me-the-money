using System;
using System.Linq;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Services;

public class FleaPriceService
{
    private static readonly Lazy<FleaPriceService> instance = new(() => new FleaPriceService());

    private FleaPriceService() { }

    public bool GetFleaPrice(TradeItem tradeItem, bool includeTaxInPrices)
    {
        if (RagfairPriceTableService.Instance.Prices is not null)
        {
            if (RagfairPriceTableService.Instance.Prices!.TryGetValue(tradeItem.Item.TemplateId, out double fleaSingleObjectPrice))
            {
                int minFleaPrice = (int)Math.Round(fleaSingleObjectPrice * GetPriceRangeMin()
                    * (double)Plugin.Configuration!.FleaPriceMultiplier.GetValue());

                double? singleObjectTaxPrice = null;
                double? totalTaxPrice = null;

                if (Plugin.Configuration!.IncludeFleaTax || Plugin.Configuration!.ShowFleaTax)
                {
                    singleObjectTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, 1, minFleaPrice, false);
                    totalTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, tradeItem.ItemObjectCount, minFleaPrice, false);
                }

                TradePrice tradePrice =
                    new(
                        tradeItem,

                        null,
                        GetFleaMarketName(),

                        minFleaPrice,
                        null,

                        null,
                        null,

                        singleObjectTaxPrice,
                        totalTaxPrice,

                        includeTaxInPrices
                    );

                tradeItem.FleaPrice = tradePrice;
            }
        }

        return tradeItem.FleaPrice is not null;
    }

    private static double GetPriceRangeMin()
    {
        return RagfairPriceRangesService.Instance.Ranges?.Default?.Min ?? 0.8d;
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