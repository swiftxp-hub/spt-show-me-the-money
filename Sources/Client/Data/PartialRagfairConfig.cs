using System.Collections.Generic;
using EFT;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Data;

public record PartialRagfairConfig
{
    public int Base { get; set; }

    public Dictionary<MongoID, double>? ItemPriceMultiplier { get; set; }

    public int MaxSellChancePercent { get; set; }

    public double SellMultiplier { get; set; }
}
