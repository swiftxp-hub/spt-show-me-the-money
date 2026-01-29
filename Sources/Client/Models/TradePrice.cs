using EFT;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public class TradePrice
{
    private readonly TradeItem _tradeItem;

    private readonly double? _singleObjectTax;

    private readonly double? _totalTax;

    private readonly bool _includeTaxInPrices;

    public TradePrice(TradeItem tradeItem, string? traderId, string traderName, int singleObjectPrice, int? totalPrice = null,
        double? currencyCourse = null, MongoID? currencyId = null, double? singleObjectTax = null, double? totalTax = null,
        bool includeTaxInPrices = false)
    {
        _tradeItem = tradeItem;
        SingleObjectPrice = singleObjectPrice;
        TotalPrice = totalPrice;

        TraderId = traderId;
        TraderName = traderName;
        CurrencyCourse = currencyCourse;
        CurrencyId = currencyId;

        _singleObjectTax = singleObjectTax;
        _totalTax = totalTax;

        _includeTaxInPrices = includeTaxInPrices;
    }

    public double GetComparePrice()
    {
        double price = SingleObjectPrice / _tradeItem.ItemSlotCount;

        if (_includeTaxInPrices)
        {
            if (_singleObjectTax is not null)
                price -= _singleObjectTax.Value / _tradeItem.ItemSlotCount;

            else if (_totalTax is not null)
                price -= _totalTax.Value / _tradeItem.Item.StackObjectsCount / _tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetComparePriceInRouble()
    {
        double price = SingleObjectPrice / _tradeItem.ItemSlotCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (_includeTaxInPrices)
        {
            if (_singleObjectTax is not null)
                price -= _singleObjectTax.Value / _tradeItem.ItemSlotCount;

            else if (_totalTax is not null)
                price -= _totalTax.Value / _tradeItem.Item.StackObjectsCount / _tradeItem.ItemSlotCount;
        }

        return price;
    }

    public double GetTotalPrice()
    {
        double? price = TotalPrice;

        if (TotalPrice is null)
            price = SingleObjectPrice * _tradeItem.Item.StackObjectsCount;

        if (_includeTaxInPrices)
        {
            if (_totalTax is not null)
                price -= _totalTax.Value;

            else if (_singleObjectTax is not null)
                price -= _singleObjectTax.Value * _tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalPriceInRouble()
    {
        double? price = TotalPrice;

        if (TotalPrice is null)
            price = SingleObjectPrice * _tradeItem.Item.StackObjectsCount;

        if (CurrencyCourse.HasValue)
            price *= CurrencyCourse.Value;

        if (_includeTaxInPrices)
        {
            if (_totalTax is not null)
                price -= _totalTax.Value;

            else if (_singleObjectTax is not null)
                price -= _singleObjectTax.Value * _tradeItem.Item.StackObjectsCount;
        }

        return price!.Value;
    }

    public double GetTotalTax()
    {
        double? tax = _totalTax;

        if (_totalTax is null)
            tax = _singleObjectTax * _tradeItem.Item.StackObjectsCount;

        return tax!.Value;
    }

    public bool HasTax()
    {
        return _singleObjectTax.HasValue || _totalTax.HasValue;
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