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
        this.SingleObjectPrice = singleObjectPrice;
        this.TotalPrice = totalPrice;

        this.TraderId = traderId;
        this.TraderName = traderName;
        this.CurrencyCourse = currencyCourse;
        this.CurrencyId = currencyId;

        this.singleObjectTax = singleObjectTax;
        this.totalTax = totalTax;

        this.includeTaxInPrices = includeTaxInPrices;
    }

    public double GetComparePrice()
    {
        double price = this.SingleObjectPrice / this.tradeItem.ItemSlotCount;

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
        double price = this.SingleObjectPrice / this.tradeItem.ItemSlotCount;

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
        double? price = this.TotalPrice;

        if (this.TotalPrice is null)
            price = this.SingleObjectPrice * this.tradeItem.Item.StackObjectsCount;

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
        double? price = this.TotalPrice;

        if (this.TotalPrice is null)
            price = this.SingleObjectPrice * this.tradeItem.Item.StackObjectsCount;

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
            if (this.CurrencyId.HasValue)
                return GClass3130.GetCurrencyCharById(this.CurrencyId.Value);

            return "₽";
        }
    }
}