using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;
using UnityEngine;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class PartialRagfairConfigService
{
    private const string RemotePathToGetPartialRagfairConfig = "/showMeTheMoney/getPartialRagfairConfig";

    private static readonly Lazy<PartialRagfairConfigService> instance = new(() => new PartialRagfairConfigService());

    private PartialRagfairConfigService() { }

    public IEnumerator GetPartialRagfairConfig()
    {
        Task<string> getJsonTask = RequestHandler.GetJsonAsync(RemotePathToGetPartialRagfairConfig);
        yield return new WaitUntil(() => getJsonTask.IsCompleted);

        string json = getJsonTask.Result;
        if (!string.IsNullOrWhiteSpace(json))
            PartialRagfairConfig = JsonConvert.DeserializeObject<PartialRagfairConfig>(json);
    }

    public static PartialRagfairConfigService Instance => instance.Value;

    public PartialRagfairConfig? PartialRagfairConfig { get; private set; }
}