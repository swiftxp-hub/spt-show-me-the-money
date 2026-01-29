using EFT;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public class TradePrice
{
    private readonly TradeItem tradeItem;

    private readonly double? singleObjectTax;

    private readonly double? totalTax;

    private readonly bool includeTaxInPrices;

    public TradePrice(TradeItem tradeItem, string? traderId, string traderName, int singleObjectPrice, int? totalPrice = null,
        double? currencyCourse = null, MongoID? currencyId = null, double? singleObjectTax = null, double? totalTax = null,
        bool includeTaxInPrices = false)
    {
        this.tradeItem = tradeItem;
        SingleObjectPrice = singleObjectPrice;
        TotalPrice = totalPrice;

        TraderId = traderId;
        TraderName = traderName;
        CurrencyCourse = currencyCourse;
        CurrencyId = currencyId;

        this.singleObjectTax = singleObjectTax;
        this.totalTax = totalTax;

        this.includeTaxInPrices = includeTaxInPrices;
    }

    public double GetComparePrice()
    {
        double price = SingleObjectPrice / tradeItem.ItemSlotCount;

        if (includeTaxInPrices)
        {
            if (singleObjectTax is not null)
                price -= singleObjectTax.Value / tradeItem.ItemSlotCount;

            else if (totalTax is not null)
                price -= totalTax.Value / tradeItem.Item.StackObjectsCount / tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetComparePriceInRouble()
    {
        double price = SingleObjectPrice / tradeItem.ItemSlotCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (includeTaxInPrices)
        {
            if (singleObjectTax is not null)
                price -= singleObjectTax.Value / tradeItem.ItemSlotCount;

            else if (totalTax is not null)
                price -= totalTax.Value / tradeItem.Item.StackObjectsCount / tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetTotalPrice()
    {
        double? price = TotalPrice;

        if (TotalPrice is null)
            price = SingleObjectPrice * tradeItem.Item.StackObjectsCount;

        if (includeTaxInPrices)
        {
            if (totalTax is not null)
                price -= totalTax.Value;

            else if (singleObjectTax is not null)
                price -= singleObjectTax.Value * tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalPriceInRouble()
    {
        double? price = TotalPrice;

        if (TotalPrice is null)
            price = SingleObjectPrice * tradeItem.Item.StackObjectsCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (includeTaxInPrices)
        {
            if (totalTax is not null)
                price -= totalTax.Value;

            else if (singleObjectTax is not null)
                price -= singleObjectTax.Value * tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalTax()
    {
        double? tax = totalTax;

        if (totalTax is null)
            tax = singleObjectTax * tradeItem.Item.StackObjectsCount;

        return tax!.Value;
    }

    public bool HasTax()
    {
        return singleObjectTax.HasValue || totalTax.HasValue;
    }

    public int SingleObjectPrice { get; private set; }

    public int? TotalPrice { get; private set; }

    public string? TraderId { get; private set; }

    public string TraderName { get; private set; }

    public double? CurrencyCourse { get; private set; }

    public MongoID? CurrencyId { get; private set; }

    public string CurrencySymbol
    {
        get
        {
            if (CurrencyId.HasValue)
                return GClass3130.GetCurrencyCharById(CurrencyId.Value);

            return "₽";
        }
    }
}