using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.EFT;
using SwiftXP.SPT.Common.Loggers.Interfaces;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class FleaPriceUpdaterService(ISimpleSptLogger simpleSptLogger)
{
    public async Task ContinuouslyUpdateFleaPricesAsync(CancellationToken cancellationToken)
    {
        TimeSpan interval = TimeSpan.FromSeconds(5);
        JsonSerializer serializer = new();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_forceUpdate ||
                    ((!EFTHelper.IsInRaid || PluginContextDataHolder.Current!.Configuration!.UpdateDuringRaid.IsEnabled())
                        && (FleaPriceDataHolder.Current == null
                            || (DateTimeOffset.UtcNow - FleaPriceDataHolder.Current.Timestamp).TotalMinutes >= PluginContextDataHolder.Current!.Configuration!.UpdateInterval.GetValue())))
                {
                    _forceUpdate = false;

                    using HttpRequestMessage requestMessage =
                        RequestHandler.HttpClient.CreateNewHttpRequest(HttpMethod.Get, Constants.RemotePathToGetFleaPrices);

                    using HttpResponseMessage response = await RequestHandler.HttpClient.HttpClient.SendAsync(requestMessage, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    using Stream contentStream = await response.Content.ReadAsStreamAsync();
                    using StreamReader streamReader = new(contentStream);

                    using JsonTextReader jsonTextReader = new(streamReader);
                    jsonTextReader.FloatParseHandling = FloatParseHandling.Double;

                    Dictionary<string, double>? fleaPrices = await Task.Run(() =>
                    {
                        return serializer.Deserialize<Dictionary<string, double>>(jsonTextReader);
                    });

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (fleaPrices != null)
                        FleaPriceDataHolder.UpdateData(fleaPrices);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                simpleSptLogger.LogException(ex);
            }
            finally
            {
                await Task.Delay(interval, cancellationToken);
            }
        }
    }

    public void ForceUpdate()
    {
        _forceUpdate = true;
    }

    private bool _forceUpdate;
}