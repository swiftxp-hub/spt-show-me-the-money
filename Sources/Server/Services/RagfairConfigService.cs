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

        PartialRagfairConfig partialRagfairConfig = new()
        {
            ItemPriceMultiplier = ragfairConfig.Dynamic.ItemPriceMultiplier,

            Base = ragfairConfig.Sell.Chance.Base,
            MaxSellChancePercent = ragfairConfig.Sell.Chance.MaxSellChancePercent,
            SellMultiplier = ragfairConfig.Sell.Chance.SellMultiplier
        };

        return partialRagfairConfig;
    }
}