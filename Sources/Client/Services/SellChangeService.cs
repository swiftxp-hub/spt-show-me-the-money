
using System;
using EFT;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class SellChangeService
{
    public static double GetPriceForDesiredSellChange(MongoID itemTemplateId, double averageOfferPrice, double qualityModifier, int desiredSellChance = -1)
    {
        if (PartialRagfairConfigService.Instance.PartialRagfairConfig?.ItemPriceMultiplier?.TryGetValue(itemTemplateId, out double itemPriceModifer) ?? false)
            averageOfferPrice *= itemPriceModifer;

        if (desiredSellChance == -1d)
            desiredSellChance = PartialRagfairConfigService.Instance.PartialRagfairConfig?.MaxSellChancePercent ?? 100;

        double sellModifier = (PartialRagfairConfigService.Instance.PartialRagfairConfig?.Base ?? 50) * qualityModifier;
        double result = averageOfferPrice
            * qualityModifier
            * (PartialRagfairConfigService.Instance.PartialRagfairConfig?.SellMultiplier ?? 1.24d)
            / Math.Pow((desiredSellChance - 10) / sellModifier, 0.25);

        return result;
    }
}