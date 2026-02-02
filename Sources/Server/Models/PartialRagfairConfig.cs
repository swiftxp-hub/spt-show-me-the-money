using System.Collections.Generic;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Models;

public record PartialRagfairConfig
{
    public int Base { get; set; }

    public Dictionary<string, double>? ItemPriceMultiplier { get; set; }

    public int MaxSellChancePercent { get; set; }

    public double SellMultiplier { get; set; }
}
