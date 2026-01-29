using System.Linq;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;
using SwiftXP.SPT.ShowMeTheMoney.Server.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class RagfairConfigService(ConfigServer configServer)
{
    public PartialRagfairConfig Get()
    {
        RagfairConfig ragfairConfig = configServer.GetConfig<RagfairConfig>();
        System.Collections.Generic.Dictionary<string, double> itemPriceMultipliers
            = ragfairConfig.Dynamic.ItemPriceMultiplier?.ToDictionary(x => x.Key.ToString(), x => x.Value) ?? [];

        PartialRagfairConfig partialRagfairConfig = new()
        {
            ItemPriceMultiplier = itemPriceMultipliers,

            Base = ragfairConfig.Sell.Chance.Base,
            MaxSellChancePercent = ragfairConfig.Sell.Chance.MaxSellChancePercent,
            SellMultiplier = ragfairConfig.Sell.Chance.SellMultiplier
        };

        return partialRagfairConfig;
    }
}