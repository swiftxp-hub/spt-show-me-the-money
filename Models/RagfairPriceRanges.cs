using System;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.ShowMeTheMoney.Models;

public class RagfairPriceRanges
{
    public static RagfairPriceRanges Instance => instance.Value;

    private static readonly Lazy<RagfairPriceRanges> instance = new(() => new RagfairPriceRanges());

    public PriceRanges? Ranges { get; private set; }

    private RagfairPriceRanges() { }

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
            Plugin.SimpleSptLogger.LogInfo("ragfair price ranges could not be queried!");
        }
    }
}