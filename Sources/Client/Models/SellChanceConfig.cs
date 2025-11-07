public record SellChanceConfig
{
    public int Base { get; set; }

    public double SellMultiplier { get; set; }

    public int MaxSellChancePercent { get; set; }

    public int MinSellChancePercent { get; set; }
}