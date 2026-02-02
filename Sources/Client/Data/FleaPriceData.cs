using System;
using System.Collections.Generic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Data;

public record FleaPriceData
{
    public FleaPriceData(Dictionary<string, double> prices)
    {
        Prices = prices;
        Timestamp = DateTime.UtcNow;
    }

    public IReadOnlyDictionary<string, double> Prices { get; }

    public DateTime Timestamp { get; }
}