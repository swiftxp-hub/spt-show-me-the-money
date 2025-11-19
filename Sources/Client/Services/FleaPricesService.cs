using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EFT;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.EFT;
using UnityEngine;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPricesService
{
    private const string RemotePathToGetStaticPriceTable = "/showMeTheMoney/getFleaPrices";

    private WaitForSeconds coroutineIntervalWait = new(30);

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

    private static readonly Lazy<FleaPricesService> instance = new(() => new FleaPricesService());

    private FleaPricesService() { }

    public IEnumerator UpdatePrices()
    {
        while (true)
        {
            if ((!EFTHelper.IsInRaid || Plugin.Configuration!.UpdateDuringRaid.IsEnabled())
                && (this.FleaPrices == null
                    || (DateTimeOffset.Now - this.lastUpdate).TotalMinutes >= Plugin.Configuration!.UpdateInterval.GetValue()))
            {
                Task<string> getJsonTask = RequestHandler.GetJsonAsync(RemotePathToGetStaticPriceTable);
                yield return new WaitUntil(() => getJsonTask.IsCompleted);

                string fleaPricesJson = getJsonTask.Result;
                if (!string.IsNullOrWhiteSpace(fleaPricesJson))
                {
                    FleaPrices = JsonConvert.DeserializeObject<Dictionary<MongoID, double>>(fleaPricesJson);
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

    public Dictionary<MongoID, double>? FleaPrices { get; private set; }
}