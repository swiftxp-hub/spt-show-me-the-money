using Newtonsoft.Json;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public class MinMax
{
    [JsonProperty("min")]
    public double? Min { get; set; }

    [JsonProperty("max")]
    public double? Max { get; set; }
}