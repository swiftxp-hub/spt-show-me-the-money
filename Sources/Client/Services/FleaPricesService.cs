using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.EFT;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;
using UnityEngine;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPricesService
{
    private const string RemotePathToGetStaticPriceTable = "/showMeTheMoney/getFleaPrices";

    private const double UpdateAfterSeconds = 300d; // 5 minutes

    private WaitForSeconds coroutineIntervalWait = new(15);

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

    private static readonly Lazy<FleaPricesService> instance = new(() => new FleaPricesService());

    private FleaPricesService() { }

    public IEnumerator UpdatePrices()
    {
        while (true)
        {
            if (!EFTHelper.IsInRaid && (this.FleaPrices == null || (DateTimeOffset.Now - this.lastUpdate).TotalSeconds >= UpdateAfterSeconds))
            {
                Task<string> getJsonTask = RequestHandler.GetJsonAsync(RemotePathToGetStaticPriceTable);
                yield return new WaitUntil(() => getJsonTask.IsCompleted);

                string fleaPricesJson = getJsonTask.Result;
                if (!string.IsNullOrWhiteSpace(fleaPricesJson))
                {
                    FleaPrices = JsonConvert.DeserializeObject<FleaPrices>(fleaPricesJson);
                    this.lastUpdate = DateTimeOffset.Now;
                }
            }

            yield return this.coroutineIntervalWait;
        }
    }

    public void ForceUpdatePrices()
    {
        this.lastUpdate = DateTimeOffset.MinValue;
    }

    public static FleaPricesService Instance => instance.Value;

    public FleaPrices? FleaPrices { get; private set; }
}