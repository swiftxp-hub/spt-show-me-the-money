using System;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class CurrencyPurchasePricesService
{
    private const string RemotePathToGetCurrencyPurchasePrices = "/showMeTheMoney/getCurrencyPurchasePrices";

    private static readonly Lazy<CurrencyPurchasePricesService> instance = new(() => new CurrencyPurchasePricesService());

    private CurrencyPurchasePricesService() { }

    public void GetCurrencyPurchasePrices()
    {
        Plugin.SimpleSptLogger.LogInfo("Trying to query currency purchase prices from remote...");

        CurrencyPurchasePrices? currencyPurchasePrises = null;
        string json = RequestHandler.GetJson(RemotePathToGetCurrencyPurchasePrices);

        if (!string.IsNullOrWhiteSpace(json))
            currencyPurchasePrises = JsonConvert.DeserializeObject<CurrencyPurchasePrices>(json);

        if (currencyPurchasePrises is not null)
        {
            Plugin.SimpleSptLogger.LogInfo($"Currency purchase prices were queried!");

            this.CurrencyPurchasePrices = currencyPurchasePrises;
        }
        else
        {
            Plugin.SimpleSptLogger.LogInfo("Currency purchase prices could not be queried!");
        }
    }

    public static CurrencyPurchasePricesService Instance => instance.Value;

    public CurrencyPurchasePrices? CurrencyPurchasePrices { get; private set; }
}