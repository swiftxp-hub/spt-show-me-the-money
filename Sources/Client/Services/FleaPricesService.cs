using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPricesService
{
    private const string RemotePathToGetStaticPriceTable = "/showMeTheMoney/getFleaPrices";

    private const double UpdateAfterSeconds = 300d; // 5 minutes

    private static readonly Lazy<FleaPricesService> instance = new(() => new FleaPricesService());

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

    private FleaPricesService() { }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> UpdatePricesAsync(bool forceUpdate = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        try
        {
            if (this.FleaPrices == null || (DateTimeOffset.Now - this.lastUpdate).TotalSeconds >= UpdateAfterSeconds || forceUpdate == true)
            {
                SimpleSptLogger.Instance.LogInfo("Trying to query flea price table from remote...");

                FleaPrices? fleaPrices = QueryFleaPrices();
                if (fleaPrices is not null)
                {
                    SimpleSptLogger.Instance.LogInfo($"Flea prices were queried! Got {fleaPrices.Count} prices from server...");

                    this.FleaPrices = fleaPrices;
                    this.lastUpdate = DateTimeOffset.Now;

                    return true;
                }
                else
                {
                    SimpleSptLogger.Instance.LogError("Flea prices could not be queried! Is the server-mod missing? Flea-prices will not be displayed.");
                }
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }

        return false;
    }

    private static FleaPrices? QueryFleaPrices()
    {
        SimpleSptLogger.Instance.LogInfo("Trying to query flea prices from server...");

        FleaPrices? result = null;

        string? pricesJson = RequestHandler.GetJson(RemotePathToGetStaticPriceTable);

        if (!string.IsNullOrWhiteSpace(pricesJson))
            result = JsonConvert.DeserializeObject<FleaPrices>(pricesJson);

        return result;
    }

    public static FleaPricesService Instance => instance.Value;

    public FleaPrices? FleaPrices { get; private set; }
}