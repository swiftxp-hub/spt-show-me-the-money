namespace SwiftXP.SPT.ShowMeTheMoney.Server.Models;

public record CurrencyPurchasePrices
{
    public CurrencyPurchasePrices(double? eur, double? usd)
    {
        this.EUR = eur;
        this.USD = usd;
    }

    public double? EUR { get; init; }

    public double? USD { get; init; }
}