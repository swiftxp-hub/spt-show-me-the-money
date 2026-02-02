using System;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Utilities;

public static class SellChangeUtility
{
    public static double GetPriceForDesiredSellChange(double averageOfferPrice, double qualityModifier, int desiredSellChance = -1)
    {
        if (desiredSellChance == -1d)
            desiredSellChance = PartialRagfairConfigHolder.Current?.MaxSellChancePercent ?? 100;

        double sellModifier = (PartialRagfairConfigHolder.Current?.Base ?? 50) * qualityModifier;

        double result = averageOfferPrice
            * (PartialRagfairConfigHolder.Current?.SellMultiplier ?? 1.24d)
            / Math.Pow(desiredSellChance / sellModifier, 0.25);

        return result;
    }
}