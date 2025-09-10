using System;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class CurrencyPurchasePricesService
{
    public static CurrencyPurchasePricesService Instance => instance.Value;

    private static readonly Lazy<CurrencyPurchasePricesService> instance = new(() => new CurrencyPurchasePricesService());

    public CurrencyPurchasePrices? CurrencyPurchasePrices { get; private set; }

    private CurrencyPurchasePricesService() { }

    public void GetCurrencyPurchasePrices()
    {
        Plugin.SimpleSptLogger.LogInfo("Trying to query currency purchase prices from remote...");

        CurrencyPurchasePrices? currencyPurchasePrises = null;
        string json = RequestHandler.GetJson(Plugin.RemotePathToGetCurrencyPurchasePrices);

        if (!string.IsNullOrWhiteSpace(json))
            currencyPurchasePrises = JsonConvert.DeserializeObject<CurrencyPurchasePrices>(json);

        if (currencyPurchasePrises is not null)
        {
            Plugin.SimpleSptLogger.LogInfo($"Currency purchase prices were queried!");

            CurrencyPurchasePrices = currencyPurchasePrises;

            Plugin.SimpleSptLogger.LogDebug($"EUR: {currencyPurchasePrises.EUR} / USD: {currencyPurchasePrises.USD}");
        }
        else
        {
            Plugin.SimpleSptLogger.LogInfo("Currency purchase prices could not be queried!");
        }
    }
}