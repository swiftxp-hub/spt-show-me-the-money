using System;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class CurrencyPurchasePricesService
{
    private const string RemotePathToGetCurrencyPurchasePrices = "/showMeTheMoney/getCurrencyPurchasePrices";

    private static readonly Lazy<CurrencyPurchasePricesService> instance = new(() => new CurrencyPurchasePricesService());

    private CurrencyPurchasePricesService() { }

    public void GetCurrencyPurchasePrices()
    {
        SimpleSptLogger.Instance.LogInfo("Trying to query currency purchase prices from remote...");

        try
        {
            CurrencyPurchasePrices? currencyPurchasePrises = null;
            string json = RequestHandler.GetJson(RemotePathToGetCurrencyPurchasePrices);

            if (!string.IsNullOrWhiteSpace(json))
                currencyPurchasePrises = JsonConvert.DeserializeObject<CurrencyPurchasePrices>(json);

            if (currencyPurchasePrises is not null)
            {
                SimpleSptLogger.Instance.LogInfo($"Currency purchase prices were queried!");

                this.CurrencyPurchasePrices = currencyPurchasePrises;
            }
            else
            {
                SimpleSptLogger.Instance.LogError("Currency purchase prices could not be queried! Is the server-mod missing? Using 'Handbook' as a fallback.");
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }
    }

    public static CurrencyPurchasePricesService Instance => instance.Value;

    public CurrencyPurchasePrices? CurrencyPurchasePrices { get; private set; }
}