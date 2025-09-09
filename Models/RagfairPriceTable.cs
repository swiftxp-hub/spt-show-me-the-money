using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.ShowMeTheMoney.Loggers;

namespace SwiftXP.ShowMeTheMoney.Models;

public class RagfairPriceTable
{
    public static RagfairPriceTable Instance => instance.Value;

    private static readonly Lazy<RagfairPriceTable> instance = new(() => new RagfairPriceTable());

    public DateTime LastQuery { get; private set; }

    public Dictionary<string, double>? Prices { get; private set; }

    private RagfairPriceTable() { }

    public bool UpdatePrices()
    {
        SimpleStaticLogger.Instance.LogInfo("Trying to query ragfair price table from remote...");

        Dictionary<string, double>? queriedPrices = null;
        string pricesJson = RequestHandler.GetJson(Plugin.RemotePathToGetPriceTable);

        if (!string.IsNullOrWhiteSpace(pricesJson))
            queriedPrices = JsonConvert.DeserializeObject<Dictionary<string, double>>(pricesJson);

        if (queriedPrices is not null)
        {
            SimpleStaticLogger.Instance.LogInfo($"Ragfair price table was queried! Got {queriedPrices.Count} prices from remote...");

            LastQuery = DateTime.UtcNow;
            Prices = queriedPrices;

            return true;
        }
        else
        {
            SimpleStaticLogger.Instance.LogInfo("Ragfair price table could not be queried!");
        }

        return false;
    }

    public bool HasPrices()
    {
        return Prices is not null;
    }
}