using System;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class SellChangeService
{
    public static double GetPriceForDesiredSellChange(double averageOfferPrice, double qualityModifier, int desiredSellChance = -1)
    {
        if (desiredSellChance == -1d)
            desiredSellChance = PartialRagfairConfigService.Instance.PartialRagfairConfig?.MaxSellChancePercent ?? 100;

        double sellModifier = (PartialRagfairConfigService.Instance.PartialRagfairConfig?.Base ?? 50) * qualityModifier;

        double result = averageOfferPrice
            * (PartialRagfairConfigService.Instance.PartialRagfairConfig?.SellMultiplier ?? 1.24d)
            / Math.Pow(desiredSellChance / sellModifier, 0.25);

        return result;
    }
}