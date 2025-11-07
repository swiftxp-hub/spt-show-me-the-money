using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using UnityEngine;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class ConfigService
{
    private const string RemotePathToGetSellChanceConfig = "/showMeTheMoney/getSellChanceConfig";

    private static readonly Lazy<ConfigService> instance = new(() => new ConfigService());

    private ConfigService() { }

    public IEnumerator GetSellChangeConfig()
    {
        Task<string> getJsonTask = RequestHandler.GetJsonAsync(RemotePathToGetSellChanceConfig);
        yield return new WaitUntil(() => getJsonTask.IsCompleted);

        string json = getJsonTask.Result;
        if (!string.IsNullOrWhiteSpace(json))
            SellChanceConfig = JsonConvert.DeserializeObject<SellChanceConfig>(json);
    }

    public static ConfigService Instance => instance.Value;

    public SellChanceConfig? SellChanceConfig { get; private set; }
}