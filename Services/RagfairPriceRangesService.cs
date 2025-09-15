using System;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class RagfairPriceRangesService
{
    private const string RemotePathToGetRagfairConfigPriceRanges = "/showMeTheMoney/getRagfairConfigPriceRanges";

    private static readonly Lazy<RagfairPriceRangesService> instance = new(() => new RagfairPriceRangesService());

    private RagfairPriceRangesService() { }

    public void GetPriceRanges()
    {
        try
        {
            Plugin.SimpleSptLogger.LogInfo("Trying to query ragfair price ranges from remote...");

            PriceRanges? priceRanges = null;
            string priceRangesJson = RequestHandler.GetJson(RemotePathToGetRagfairConfigPriceRanges);
            Plugin.SimpleSptLogger.LogDebug($"JSON: {priceRangesJson}");

            if (!string.IsNullOrWhiteSpace(priceRangesJson))
                priceRanges = JsonConvert.DeserializeObject<PriceRanges>(priceRangesJson);

            if (priceRanges is not null)
            {
                Plugin.SimpleSptLogger.LogInfo($"Ragfair price ranges was queried!");

                this.Ranges = priceRanges;
            }
            else
            {
                Plugin.SimpleSptLogger.LogError("Ragfair price ranges could not be queried! Is the server-mod missing? Using SPT-defaults as a fallback.");
            }
        }
        catch (Exception exception)
        {
            Plugin.SimpleSptLogger.LogException(exception);
        }
    }

    public static RagfairPriceRangesService Instance => instance.Value;

    public PriceRanges? Ranges { get; private set; }
}