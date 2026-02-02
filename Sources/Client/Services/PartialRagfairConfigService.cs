using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SPT.Common.Http;
using SwiftXP.SPT.Common.Loggers.Interfaces;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Services;

public class PartialRagfairConfigService(ISimpleSptLogger simpleSptLogger)
{
    public async Task GetPartialRagfairConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            string jsonResult = await RequestHandler.GetJsonAsync(Constants.RemotePathToGetPartialRagfairConfig);

            if (cancellationToken.IsCancellationRequested)
                return;

            PartialRagfairConfig? partialRagfairConfig = await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<PartialRagfairConfig>(jsonResult);
            });

            if (cancellationToken.IsCancellationRequested)
                return;

            if (partialRagfairConfig != null)
                PartialRagfairConfigHolder.UpdateData(partialRagfairConfig);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            simpleSptLogger.LogException(ex);
        }
    }
}