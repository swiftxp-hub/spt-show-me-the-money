using System;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Enums;
using SwiftXP.SPT.ShowMeTheMoney.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Services;

public class RagfairPriceTableService
{
    private const string RemotePathToGetStaticPriceTable = "/showMeTheMoney/getStaticPriceTable";

    private const string RemotePathToGetDynamicPriceTable = "/showMeTheMoney/getDynamicPriceTable";

    private const double UpdateAfterSeconds = 300d; // 5 minutes

    private static readonly Lazy<RagfairPriceTableService> instance = new(() => new RagfairPriceTableService());

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

    private RagfairPriceTableService() { }

    public bool UpdatePrices(bool forceUpdate = false)
    {
        try
        {
            if (this.Prices == null || (DateTimeOffset.Now - this.lastUpdate).TotalSeconds >= UpdateAfterSeconds || forceUpdate == true)
            {
                SimpleSptLogger.Instance.LogInfo("Trying to query ragfair price table from remote...");

                RagfairPriceTable? queriedPrices = null;
                string? pricesJson = null;

                switch (Plugin.Configuration!.RagfairPriceTableMethod.Value)
                {
                    case RagfairPriceTableMethodEnum.Static:
                        pricesJson = RequestHandler.GetJson(RemotePathToGetStaticPriceTable);
                        break;

                    default:
                        pricesJson = RequestHandler.GetJson(RemotePathToGetDynamicPriceTable);
                        break;
                }

                if (!string.IsNullOrWhiteSpace(pricesJson))
                    queriedPrices = JsonConvert.DeserializeObject<RagfairPriceTable>(pricesJson);

                if (queriedPrices is not null)
                {
                    SimpleSptLogger.Instance.LogInfo($"Ragfair price table was queried! Got {queriedPrices.Count} prices from remote...");

                    this.Prices = queriedPrices;
                    this.lastUpdate = DateTimeOffset.Now;

                    return true;
                }
                else
                {
                    SimpleSptLogger.Instance.LogError("Ragfair price table could not be queried! Is the server-mod missing? Flea-prices will not be displayed.");
                }
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