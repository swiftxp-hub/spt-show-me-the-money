using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

    private WaitForSeconds _coroutineIntervalWait = new(30);

    private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

    private static readonly Lazy<FleaPricesService> s_instance = new(() => new FleaPricesService());

    private FleaPricesService() { }

    public IEnumerator UpdatePrices()
    {
        while (true)
        {
            if ((!EFTHelper.IsInRaid || Plugin.Configuration!.UpdateDuringRaid.IsEnabled())
                && (FleaPrices == null
                    || (DateTimeOffset.Now - _lastUpdate).TotalMinutes >= Plugin.Configuration!.UpdateInterval.GetValue()))
            {
                HttpRequestMessage httpRequestMessage =
                    RequestHandler.HttpClient.CreateNewHttpRequest(HttpMethod.Get, RemotePathToGetStaticPriceTable);

                Task<HttpResponseMessage> requestTask = RequestHandler.HttpClient.HttpClient.SendAsync(httpRequestMessage);
                yield return new WaitUntil(() => requestTask.IsCompleted);

                HttpResponseMessage response = requestTask.Result;
                response.EnsureSuccessStatusCode();

                using Task<Stream> readAsStreamTask = response.Content.ReadAsStreamAsync();
                yield return new WaitUntil(() => readAsStreamTask.IsCompleted);

                using Stream contentStream = readAsStreamTask.Result;
                using StreamReader streamReader = new(contentStream);
                using JsonTextReader jsonTextReader = new(streamReader);

                JsonSerializer serializer = new();
                jsonTextReader.FloatParseHandling = FloatParseHandling.Double;

                FleaPrices = serializer.Deserialize<Dictionary<MongoID, double>>(jsonTextReader);
                _lastUpdate = DateTimeOffset.Now;
            }

            yield return _coroutineIntervalWait;
        }
    }

    public void ForceUpdatePrices()
    {
        _lastUpdate = DateTimeOffset.MinValue;
    }

    public static FleaPricesService Instance => s_instance.Value;

    public Dictionary<MongoID, double>? FleaPrices { get; private set; }
}