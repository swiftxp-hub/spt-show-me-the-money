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

    private static StaticFleaPriceTableService? StaticFleaPriceTableService;

    private static DynamicFleaPriceTableService? DynamicFleaPriceTableService;

    public ShowMeTheMoneyStaticRouter(JsonUtil jsonUtil, StaticFleaPriceTableService staticFleaPriceTableService, DynamicFleaPriceTableService dynamicFleaPriceTableService)
        : base(jsonUtil, GetRoutes())
    {
        JsonUtil = jsonUtil;

        StaticFleaPriceTableService = staticFleaPriceTableService;
        DynamicFleaPriceTableService = dynamicFleaPriceTableService;
    }

    private static List<RouteAction> GetRoutes()
    {
        return
        [
            new RouteAction(
                "/showMeTheMoney/getStaticFleaPriceTable",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetStaticFleaPriceTable()
            ),

            new RouteAction(
                "/showMeTheMoney/getDynamicFleaPriceTable",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await GetDynamicFleaPriceTable()
            )
        ];
    }

    private static async ValueTask<string> GetStaticFleaPriceTable()
    {
        ConcurrentDictionary<MongoId, double> result = StaticFleaPriceTableService!.Get();

        return JsonUtil!.Serialize(result)!;
    }

    private static async ValueTask<string> GetDynamicFleaPriceTable()
    {
        ConcurrentDictionary<MongoId, double> result = DynamicFleaPriceTableService!.Get();

        return JsonUtil!.Serialize(result)!;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously