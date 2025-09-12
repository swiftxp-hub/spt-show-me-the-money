using EFT;
using CurrencyUtility = GClass2934;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class TradePrice
{
    private readonly TradeItem tradeItem;

    private readonly double singleObjectPrice;

    private readonly double? totalPrice;

    private readonly double? singleObjectTax;

    private readonly double? totalTax;

    private readonly bool includeTaxInPrices;

    public TradePrice(TradeItem tradeItem, string traderName, double singleObjectPrice, double? totalPrice = null,
        double? currencyCourse = null, MongoID? currencyId = null, double? singleObjectTax = null, double? totalTax = null,
        bool includeTaxInPrices = false)
    {
        this.tradeItem = tradeItem;
        this.singleObjectPrice = singleObjectPrice;
        this.totalPrice = totalPrice;

        this.TraderName = traderName;
        this.CurrencyCourse = currencyCourse;
        this.CurrencyId = currencyId;

        this.singleObjectTax = singleObjectTax;
        this.totalTax = totalTax;

        this.includeTaxInPrices = includeTaxInPrices;
    }

    public double GetComparePrice()
    {
        double price = this.singleObjectPrice / this.tradeItem.ItemSlotCount;

        if (this.includeTaxInPrices)
        {
            if (this.singleObjectTax is not null)
                price -= this.singleObjectTax.Value / this.tradeItem.ItemSlotCount;

            else if (this.totalTax is not null)
                price -= this.totalTax.Value / this.tradeItem.Item.StackObjectsCount / this.tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetComparePriceInRouble()
    {
        double price = this.singleObjectPrice / this.tradeItem.ItemSlotCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (this.includeTaxInPrices)
        {
            if (this.singleObjectTax is not null)
                price -= this.singleObjectTax.Value / this.tradeItem.ItemSlotCount;

            else if (this.totalTax is not null)
                price -= this.totalTax.Value / this.tradeItem.Item.StackObjectsCount / this.tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetTotalPrice()
    {
        double? price = this.totalPrice;

        if (this.totalPrice is null)
            price = this.singleObjectPrice * this.tradeItem.Item.StackObjectsCount;

        if (this.includeTaxInPrices)
        {
            if (this.totalTax is not null)
                price -= this.totalTax.Value;

            else if (this.singleObjectTax is not null)
                price -= this.singleObjectTax.Value * this.tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalPriceInRouble()
    {
        double? price = this.totalPrice;

        if (this.totalPrice is null)
            price = this.singleObjectPrice * this.tradeItem.Item.StackObjectsCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (this.includeTaxInPrices)
        {
            if (this.totalTax is not null)
                price -= this.totalTax.Value;

            else if (this.singleObjectTax is not null)
                price -= this.singleObjectTax.Value * this.tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalTax()
    {
        double? tax = this.totalTax;

        if (this.totalTax is null)
            tax = this.singleObjectTax * this.tradeItem.Item.StackObjectsCount;

        return tax!.Value;
    }

    public bool HasTax()
    {
        return this.singleObjectTax.HasValue || this.totalTax.HasValue;
    }

    public string TraderName { get; }

    public double? CurrencyCourse { get; set; }

    public MongoID? CurrencyId { get; set; }

    public string CurrencySymbol
    {
        get
        {
            if (this.CurrencyId.HasValue)
                return CurrencyUtility.GetCurrencyCharById(this.CurrencyId.Value);

            return "₽";
        }
    }
}