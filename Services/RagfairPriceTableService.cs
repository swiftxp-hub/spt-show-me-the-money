using System;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class RagfairPriceTableService
{
    private const string RemotePathToGetPriceTable = "/showMeTheMoney/getPriceTable";

    private static readonly Lazy<RagfairPriceTableService> instance = new(() => new RagfairPriceTableService());

    private RagfairPriceTableService() { }

    public bool UpdatePrices()
    {
        try
        {
            SimpleSptLogger.Instance.LogInfo("Trying to query ragfair price table from remote...");

            RagfairPriceTable? queriedPrices = null;
            string pricesJson = RequestHandler.GetJson(RemotePathToGetPriceTable);

            if (!string.IsNullOrWhiteSpace(pricesJson))
                queriedPrices = JsonConvert.DeserializeObject<RagfairPriceTable>(pricesJson);

            if (queriedPrices is not null)
            {
                SimpleSptLogger.Instance.LogInfo($"Ragfair price table was queried! Got {queriedPrices.Count} prices from remote...");

                this.Prices = queriedPrices;

                return true;
            }
            else
            {
                SimpleSptLogger.Instance.LogError("Ragfair price table could not be queried! Is the server-mod missing? Flea-prices will not be displayed.");
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }

        return false;
    }

    public static RagfairPriceTableService Instance => instance.Value;

    public RagfairPriceTable? Prices { get; private set; }
}