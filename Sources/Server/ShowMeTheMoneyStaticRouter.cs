#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Utils;
using SwiftXP.SPT.ShowMeTheMoney.Server.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server;

[Injectable]
public class ShowMeTheMoneyStaticRouter : StaticRouter
{
    private static JsonUtil? JsonUtil;

    private static FleaPricesService? FleaPricesService;

    private static RagfairConfigService? RagfairConfigService;

    public ShowMeTheMoneyStaticRouter(JsonUtil jsonUtil, RagfairConfigService ragfairConfigService, FleaPricesService fleaPricesService)
        : base(jsonUtil, GetRoutes())
    {
        JsonUtil = jsonUtil;

        FleaPricesService = fleaPricesService;
        RagfairConfigService = ragfairConfigService;
    }

    private static List<RouteAction> GetRoutes()
    {
        return
        [
            new RouteAction(
                "/showMeTheMoney/getFleaPrices",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetFleaPrices()
            ),

            new RouteAction(
                "/showMeTheMoney/getPartialRagfairConfig",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetPartialRagfairConfig()
            )
        ];
    }

    private static async ValueTask<string> GetFleaPrices()
    {
        ConcurrentDictionary<MongoId, double> result = FleaPricesService!.Get();

        return JsonUtil!.Serialize(result)!;
    }

    private static async ValueTask<string> GetPartialRagfairConfig()
    {
        Models.PartialRagfairConfig result = RagfairConfigService!.Get();

        return JsonUtil!.Serialize(result)!;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously