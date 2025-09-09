using Newtonsoft.Json;

namespace SwiftXP.ShowMeTheMoney.Models;

public class PriceRanges
{
    [JsonProperty("default")]
    public MinMax? Default { get; set; }

    [JsonProperty("preset")]
    public MinMax? Preset { get; set; }

    [JsonProperty("pack")]
    public MinMax? Pack { get; set; }
}