
using System;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public static class SellChangeService
{
    public static double GetPriceForDesiredSellChange(double averageOfferPrice, double qualityModifier, double desiredSellChance = 90d)
    {
        double x = 50 * qualityModifier;
        double result = averageOfferPrice * qualityModifier * 1.24 / Math.Pow((desiredSellChance - 10) / x, 0.25);

        return result;
    }
}