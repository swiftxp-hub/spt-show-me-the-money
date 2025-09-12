using System;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class RagfairPriceTableService
{
    private const string RemotePathToGetPriceTable = "/showMeTheMoney/getPriceTable";

    private static readonly Lazy<RagfairPriceTableService> instance = new(() => new RagfairPriceTableService());

    private RagfairPriceTableService() { }

    public bool UpdatePrices()
    {
        Plugin.SimpleSptLogger.LogInfo("Trying to query ragfair price table from remote...");

        RagfairPriceTable? queriedPrices = null;
        string pricesJson = RequestHandler.GetJson(RemotePathToGetPriceTable);

        if (!string.IsNullOrWhiteSpace(pricesJson))
            queriedPrices = JsonConvert.DeserializeObject<RagfairPriceTable>(pricesJson);

        if (queriedPrices is not null)
        {
            Plugin.SimpleSptLogger.LogInfo($"Ragfair price table was queried! Got {queriedPrices.Count} prices from remote...");

            this.Prices = queriedPrices;

            return true;
        }
        else
        {
            Plugin.SimpleSptLogger.LogInfo("Ragfair price table could not be queried!");
        }

        return false;
    }

    public static RagfairPriceTableService Instance => instance.Value;

    public RagfairPriceTable? Prices { get; private set; }
}