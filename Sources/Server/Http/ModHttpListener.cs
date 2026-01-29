using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SwiftXP.SPT.ShowMeTheMoney.Server.Http.Interfaces;
using SwiftXP.SPT.ShowMeTheMoney.Server.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Http;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class ModHttpListener(
    ISptLogger<ModHttpListener> logger,
    FleaPricesService fleaPricesService,
    RagfairConfigService ragfairConfigService)

    : IModHttpListener
{
    public bool CanHandle(MongoId sessionId, HttpContext context)
    {
        return context.Request.Path.StartsWithSegments(Constants.RoutePrefix, StringComparison.OrdinalIgnoreCase);
    }

    public async Task Handle(MongoId sessionId, HttpContext context)
    {
        try
        {
            string requestPath = context.Request.Path.Value ?? string.Empty;

            if (IsRoute(requestPath, Constants.RouteGetFleaPrices))
            {
                await HandleGetFleaPricesAsync(context);
            }
            else if (IsRoute(requestPath, Constants.RouteGetPartialRagfairConfig))
            {
                await HandleGetPartialRagfairConfigAsync(context);
            }
            else
            {
                await HandleUnknownRouteAsync(context, requestPath);
            }
        }
        catch (Exception ex)
        {
            logger.Error($"[Show Me The Money] Error handling request: {ex.Message}");

            context.Response.StatusCode = 500;
        }
    }

    private async Task HandleGetFleaPricesAsync(HttpContext context)
    {
        ConcurrentDictionary<string, double> result = fleaPricesService!.Get();

        await context.Response.WriteAsJsonAsync(result, context.RequestAborted);
    }

    private async Task HandleGetPartialRagfairConfigAsync(HttpContext context)
    {
        Models.PartialRagfairConfig result = ragfairConfigService!.Get();

        await context.Response.WriteAsJsonAsync(result, context.RequestAborted);
    }

#pragma warning disable CS1998 // This async method lacks 'await' operators.
    private async Task HandleUnknownRouteAsync(HttpContext context, string requestPath)
#pragma warning restore CS1998 // This async method lacks 'await' operators.
    {
        logger.Warning($"[Show Me The Money] Unknown route: {requestPath}");

        context.Response.StatusCode = 404;
    }

    private static bool IsRoute(string path, string subRoute)
    {
        return path.Equals($"{Constants.RoutePrefix}{subRoute}", StringComparison.OrdinalIgnoreCase);
    }
}