using System;
using System.Collections.Generic;
using System.Linq;
using EFT.InventoryLogic;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPriceService
{
    private static readonly Lazy<FleaPriceService> instance = new(() => new FleaPriceService());

    private FleaPriceService() { }

    public bool GetFleaPrice(TradeItem tradeItem, bool includeTaxInPrices)
    {
        if (tradeItem.Item.CanSellOnRagfair && FleaPricesService.Instance.FleaPrices != null)
        {
            List<double> serverFleaPrices = [];
            if (GetFleaPriceForItem(tradeItem.Item, out double? fleaPriceForBaseItem))
                serverFleaPrices.Add(fleaPriceForBaseItem!.Value);

            if (tradeItem.Item.TryGetItemComponent(out ArmorHolderComponent armorHolderComponent))
            {
                Plugin.SptLogger!.LogInfo("Is Armor...");

                foreach (ArmorPlateItemClass armorPlateItem in armorHolderComponent.MoveAbleArmorPlates)
                {
                    if (GetFleaPriceForItem(armorPlateItem, out double? fleaPriceForPlate))
                    {
                        serverFleaPrices.Add(fleaPriceForPlate!.Value);
                        Plugin.SptLogger!.LogInfo("Armor-Plate...");
                    }
                }
            }

            double sum = serverFleaPrices.Sum();

            if (sum > 0d)
                SetFleaPriceOfTradeItem(tradeItem, sum, includeTaxInPrices);
        }

        return tradeItem.FleaPrice is not null;
    }

    private bool GetFleaPriceForItem(Item item, out double? fleaPriceForItem)
    {
        fleaPriceForItem = null;

        double? fleaPrice = FleaPricesService.Instance.FleaPrices?.GetValueOrDefault(item.TemplateId);
        if (fleaPrice.HasValue)
        {
            double priceQualityModifier = ItemQualityService.GetItemQualityModifier(item);
            fleaPriceForItem = fleaPrice.Value * priceQualityModifier;

            return true;
        }

        return false;
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