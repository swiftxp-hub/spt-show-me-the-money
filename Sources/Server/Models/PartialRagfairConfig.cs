using System.Collections.Generic;
using SPTarkov.Server.Core.Models.Common;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Models;

public record PartialRagfairConfig
{
    public int Base { get; set; }

    public Dictionary<MongoId, double>? ItemPriceMultiplier { get; set; }

    public int MaxSellChancePercent { get; set; }

    public double SellMultiplier { get; set; }
}
