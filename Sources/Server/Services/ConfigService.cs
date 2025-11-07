using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Servers;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class ConfigService(ConfigServer configServer)
{
    public Chance GetSellChanceConfig()
    {
        RagfairConfig ragfairConfig = configServer.GetConfig<RagfairConfig>();

        return ragfairConfig.Sell.Chance;
    }
}