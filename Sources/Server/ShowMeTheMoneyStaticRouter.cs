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

    private static FleaPriceService? FleaPriceService;

    public ShowMeTheMoneyStaticRouter(JsonUtil jsonUtil, FleaPriceService fleaPriceService)
        : base(jsonUtil, GetRoutes())
    {
        JsonUtil = jsonUtil;

        FleaPriceService = fleaPriceService;
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
            )
        ];
    }

    private static async ValueTask<string> GetFleaPrices()
    {
        ConcurrentDictionary<MongoId, double> result = FleaPriceService!.Get();

        return JsonUtil!.Serialize(result)!;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously