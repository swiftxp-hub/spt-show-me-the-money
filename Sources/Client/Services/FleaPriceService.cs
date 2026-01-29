using System;
using System.Linq;
using EFT.InventoryLogic;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPriceService
{
    public static bool GetFleaPrice(TradeItem tradeItem, bool includeTaxInPrices)
    {
        if (tradeItem.Item.CanSellOnRagfair && FleaPricesService.Instance.FleaPrices != null)
        {
            if (FleaPricesService.Instance.FleaPrices?.TryGetValue(tradeItem.Item.TemplateId, out double fleaPrice) ?? false)
            {
                double qualityModifier = ItemQualityService.GetItemQualityModifier(tradeItem.Item);

                // Handle weapons
                if (tradeItem.Item is Weapon weapon)
                {
                    fleaPrice = GetWeaponPrice(weapon, fleaPrice);
                }
                // Handle everything else
                else
                {
                    if (PartialRagfairConfigService.Instance.PartialRagfairConfig?.ItemPriceMultiplier?.TryGetValue(tradeItem.Item.TemplateId, out double itemPriceModifer) ?? false)
                        fleaPrice *= itemPriceModifer;
                }

                fleaPrice *= qualityModifier;

                double bestFleaPrice = SellChangeService.GetPriceForDesiredSellChange(fleaPrice, qualityModifier);

                if (bestFleaPrice > 0d)
                    SetFleaPriceOfTradeItem(tradeItem, bestFleaPrice, includeTaxInPrices);
            }
        }

        return tradeItem.FleaPrice is not null;
    }

    private static double GetWeaponPrice(Weapon weapon, double staticWeaponPrice)
    {
        double totalWeaponPrice = staticWeaponPrice;

        foreach (Mod mod in weapon.Mods)
        {
            if (FleaPricesService.Instance.FleaPrices?.TryGetValue(mod.TemplateId, out double fleaPrice) ?? false)
                totalWeaponPrice += fleaPrice;
        }

        return totalWeaponPrice;
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
}