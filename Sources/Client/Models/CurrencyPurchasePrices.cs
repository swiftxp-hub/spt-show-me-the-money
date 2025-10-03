using Newtonsoft.Json;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public class CurrencyPurchasePrices
{
    [JsonProperty("eur")]
    public double? EUR { get; set; }

    [JsonProperty("usd")]
    public double? USD { get; set; }
}