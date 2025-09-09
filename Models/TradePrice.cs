namespace SwiftXP.ShowMeTheMoney.Models;

public class TradePrice
{
    public TradePrice(string traderName, double pricePerSlot, double pricePerObject, double priceTotal, double? tax = null)
    {
        TraderName = traderName;
        PricePerSlot = pricePerSlot;
        PricePerObject = pricePerObject;
        PriceTotal = priceTotal;
        Tax = tax;
    }

    public string TraderName { get; }

    public double PricePerSlot { get; }

    public double PricePerObject { get; }

    public double PriceTotal { get; }

    public double? Tax { get;  }
}