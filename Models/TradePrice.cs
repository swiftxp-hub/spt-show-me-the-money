using EFT;
using CurrencyUtility = GClass2934;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class TradePrice
{
    public string TraderName { get; }

    public double Price { get; set; }

    public double? CurrencyCourse { get; set; }

    public MongoID? CurrencyId { get; set; }

    public double? Tax { get; set; }

    public string CurrencySymbol
    {
        get
        {
            if (CurrencyId.HasValue)
                return CurrencyUtility.GetCurrencyCharById(CurrencyId.Value);

            return "₽";
        }
    }

    public TradePrice(string traderName, double price, double? currencyCourse = null, MongoID? currencyId = null, double? tax = null)
    {
        TraderName = traderName;
        Price = price;
        CurrencyCourse = currencyCourse;
        CurrencyId = currencyId;
        Tax = tax;
    }

    public double GetSlotPrice(int itemSlotCount)
    {
        return Price / itemSlotCount;
    }

    public double GetSlotPriceInRouble(int itemSlotCount)
    {
        double totalPriceInRouble = GetTotalPriceInRouble();

        return totalPriceInRouble / itemSlotCount;
    }

    public double GetObjectPriceInRouble(int stackObjectsCount)
    {
        double totalPriceInRouble = GetTotalPriceInRouble();

        return totalPriceInRouble / stackObjectsCount;
    }

    public double GetTotalPriceInRouble()
    {
        if (CurrencyCourse.HasValue)
            return Price * CurrencyCourse.Value;

        return Price;
    }
}