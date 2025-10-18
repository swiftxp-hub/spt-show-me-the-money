using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPriceTableService
{
    private const string RemotePathToGetStaticPriceTable = "/showMeTheMoney/getFleaPrices";

    private const double UpdateAfterSeconds = 1d; //300d; // 5 minutes

    private static readonly Lazy<FleaPriceTableService> instance = new(() => new FleaPriceTableService());

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

    private FleaPriceTableService() { }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> UpdatePricesAsync(bool forceUpdate = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        try
        {
            if (this.Prices == null || (DateTimeOffset.Now - this.lastUpdate).TotalSeconds >= UpdateAfterSeconds || forceUpdate == true)
            {
                SimpleSptLogger.Instance.LogInfo("Trying to query flea price table from remote...");

                FleaPriceTable? fleaPriceTable = GetFleaPriceTable();
                if (fleaPriceTable is not null)
                {
                    SimpleSptLogger.Instance.LogInfo($"Flea price table was queried! Got {fleaPriceTable.Count} prices from remote...");

                    this.Prices = fleaPriceTable;
                    this.lastUpdate = DateTimeOffset.Now;

                    return true;
                }
                else
                {
                    SimpleSptLogger.Instance.LogError("Flea price table could not be queried! Is the server-mod missing? Flea-prices will not be displayed.");
                }
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }

        return false;
    }

    private static FleaPriceTable? GetFleaPriceTable()
    {
        SimpleSptLogger.Instance.LogInfo("Trying to query static flea price table from remote...");

        FleaPriceTable? result = null;

        string? pricesJson = RequestHandler.GetJson(RemotePathToGetStaticPriceTable);

        if (!string.IsNullOrWhiteSpace(pricesJson))
            result = JsonConvert.DeserializeObject<FleaPriceTable>(pricesJson);

        return result;
    }

    public static FleaPriceTableService Instance => instance.Value;

    public FleaPriceTable? Prices { get; private set; }
}