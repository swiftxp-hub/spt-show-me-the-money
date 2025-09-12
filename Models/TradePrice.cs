using EFT;
using CurrencyUtility = GClass2934;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class TradePrice
{
    private TradeItem _tradeItem;

    private double _singleObjectPrice;

    private double? _totalPrice;

    private double? _singleObjectTax;

    private double? _totalTax;

    private bool _includeTaxInPrices;

    public string TraderName { get; }

    public double? CurrencyCourse { get; set; }

    public MongoID? CurrencyId { get; set; }

    public string CurrencySymbol
    {
        get
        {
            if (CurrencyId.HasValue)
                return CurrencyUtility.GetCurrencyCharById(CurrencyId.Value);

            return "₽";
        }
    }

    public TradePrice(TradeItem tradeItem, string traderName, double singleObjectPrice, double? totalPrice = null,
        double? currencyCourse = null, MongoID? currencyId = null, double? singleObjectTax = null, double? totalTax = null,
        bool includeTaxInPrices = false)
    {
        _tradeItem = tradeItem;
        _singleObjectPrice = singleObjectPrice;
        _totalPrice = totalPrice;

        TraderName = traderName;
        CurrencyCourse = currencyCourse;
        CurrencyId = currencyId;

        _singleObjectTax = singleObjectTax;
        _totalTax = totalTax;

        _includeTaxInPrices = includeTaxInPrices;
    }

    public double GetComparePrice()
    {
        double price = _singleObjectPrice / _tradeItem.ItemSlotCount;

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
        double price = _singleObjectPrice / _tradeItem.ItemSlotCount;

        if (CurrencyCourse.HasValue)
            price = price * CurrencyCourse.Value;

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
        double? price = _totalPrice;

        if (_totalPrice is null)
            price = _singleObjectPrice * _tradeItem.Item.StackObjectsCount;

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
        return this._singleObjectTax.HasValue || this._totalTax.HasValue;
    }
}