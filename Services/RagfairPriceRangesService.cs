using System;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class RagfairPriceRangesService
{
    public static RagfairPriceRangesService Instance => instance.Value;

    private static readonly Lazy<RagfairPriceRangesService> instance = new(() => new RagfairPriceRangesService());

    public PriceRanges? Ranges { get; private set; }

    private RagfairPriceRangesService() { }

    public void GetPriceRanges()
    {
        Plugin.SimpleSptLogger.LogInfo("Trying to query ragfair price ranges from remote...");

        PriceRanges? priceRanges = null;
        string priceRangesJson = RequestHandler.GetJson(Plugin.RemotePathToGetRagfairConfigPriceRanges);

        if (!string.IsNullOrWhiteSpace(priceRangesJson))
            priceRanges = JsonConvert.DeserializeObject<PriceRanges>(priceRangesJson);

        if (priceRanges is not null)
        {
            Plugin.SimpleSptLogger.LogInfo($"Ragfair price ranges was queried!");

            Ranges = priceRanges;
        }
        else
        {
            Plugin.SimpleSptLogger.LogInfo("Ragfair price ranges could not be queried!");
        }
    }
}