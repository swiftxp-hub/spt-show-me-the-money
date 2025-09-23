using System;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Services;

public class RagfairPriceRangesService
{
    private const string RemotePathToGetRagfairConfigPriceRanges = "/showMeTheMoney/getRagfairConfigPriceRanges";

    private static readonly Lazy<RagfairPriceRangesService> instance = new(() => new RagfairPriceRangesService());

    private RagfairPriceRangesService() { }

    public void GetPriceRanges()
    {
        try
        {
            SimpleSptLogger.Instance.LogInfo("Trying to query ragfair price ranges from remote...");

            PriceRanges? priceRanges = null;
            string priceRangesJson = RequestHandler.GetJson(RemotePathToGetRagfairConfigPriceRanges);

            if (!string.IsNullOrWhiteSpace(priceRangesJson))
                priceRanges = JsonConvert.DeserializeObject<PriceRanges>(priceRangesJson);

            if (priceRanges is not null)
            {
                SimpleSptLogger.Instance.LogInfo($"Ragfair price ranges was queried!");

                this.Ranges = priceRanges;
            }
            else
            {
                SimpleSptLogger.Instance.LogError("Ragfair price ranges could not be queried! Is the server-mod missing? Using SPT-defaults as a fallback.");
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }
    }

    public static RagfairPriceRangesService Instance => instance.Value;

    public PriceRanges? Ranges { get; private set; }
}