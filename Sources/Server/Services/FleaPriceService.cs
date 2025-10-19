using System.Collections.Generic;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class FleaPriceService(RagfairPriceService ragfairPriceService)
{
    public Dictionary<MongoId, double> Get()
    {
        Dictionary<MongoId, double> result = ragfairPriceService.GetAllFleaPrices();

        return result;
    }
}