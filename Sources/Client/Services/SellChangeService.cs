
using System;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class SellChangeService
{
    public static double GetPriceForDesiredSellChange(double averageOfferPrice, double qualityModifier, int desiredSellChance = -1)
    {
        if (desiredSellChance == -1d)
            desiredSellChance = ConfigService.Instance.SellChanceConfig?.MaxSellChancePercent ?? 100;

        double sellModifier = (ConfigService.Instance.SellChanceConfig?.Base ?? 50) * qualityModifier;
        double result = averageOfferPrice
            * qualityModifier
            * (ConfigService.Instance.SellChanceConfig?.SellMultiplier ?? 1.24d)
            / Math.Pow((desiredSellChance - 10) / sellModifier, 0.25);

        return result;
    }
}