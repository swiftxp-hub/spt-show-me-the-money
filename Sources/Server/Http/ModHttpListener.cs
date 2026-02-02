using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers.Http;
using SwiftXP.SPT.ShowMeTheMoney.Server.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Http;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class ModHttpListener(
    ISptLogger<ModHttpListener> sptLogger,
    FleaPricesService fleaPricesService,
    RagfairConfigService ragfairConfigService) : IHttpListener
{
    private static readonly PathString s_pathGetFleaPrices = new($"{Constants.RoutePrefix}{Constants.RouteGetFleaPrices}");
    private static readonly PathString s_pathGetPartialRagfairConfig = new($"{Constants.RoutePrefix}{Constants.RouteGetPartialRagfairConfig}");

    public bool CanHandle(MongoId sessionId, HttpContext context)
    {
        return context.Request.Path.StartsWithSegments(Constants.RoutePrefix, StringComparison.OrdinalIgnoreCase);
    }

    public async Task Handle(MongoId sessionId, HttpContext context)
    {
        try
        {
            PathString path = context.Request.Path;

            if (path.Equals(s_pathGetFleaPrices, StringComparison.OrdinalIgnoreCase))
            {
                await HandleGetFleaPricesAsync(context);
            }
            else if (path.Equals(s_pathGetPartialRagfairConfig, StringComparison.OrdinalIgnoreCase))
            {
                await HandleGetPartialRagfairConfigAsync(context);
            }
            else
            {
                await HandleUnknownRouteAsync(context, path.Value ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            sptLogger.Error($"{Constants.LoggerPrefix}Error handling request: {ex.Message}");

            context.Response.StatusCode = 500;
        }
    }

    private async Task HandleGetFleaPricesAsync(HttpContext context)
    {
        IReadOnlyDictionary<string, double> result = fleaPricesService.Get();

        await context.Response.WriteAsJsonAsync(result, context.RequestAborted);
    }

    private async Task HandleGetPartialRagfairConfigAsync(HttpContext context)
    {
        Models.PartialRagfairConfig result = ragfairConfigService.Get();

        await context.Response.WriteAsJsonAsync(result, context.RequestAborted);
    }

    private Task HandleUnknownRouteAsync(HttpContext context, string requestPath)
    {
        sptLogger.Warning($"{Constants.LoggerPrefix}Unknown route: {requestPath}");

        context.Response.StatusCode = 404;

        return Task.CompletedTask;
    }
}
