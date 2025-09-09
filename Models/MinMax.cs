using Newtonsoft.Json;

namespace SwiftXP.ShowMeTheMoney.Models;

public class MinMax
{
    [JsonProperty("min")]
    public double? Min { get; set; }

    [JsonProperty("max")]
    public double? Max { get; set; }
}