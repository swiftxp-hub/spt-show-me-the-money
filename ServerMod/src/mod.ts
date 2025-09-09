import type { DependencyContainer } from "tsyringe";
import type { StaticRouterModService } from "@spt/services/mod/staticRouter/StaticRouterModService";
import type { ConfigServer } from "@spt/servers/ConfigServer";
import { ConfigTypes } from "@spt/models/enums/ConfigTypes"
import { IRagfairConfig } from "@spt/models/spt/config/IRagfairConfig";
import type { DatabaseServer } from "@spt/servers/DatabaseServer";
import type { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import { ILogger } from "@spt/models/spt/utils/ILogger";

class Mod implements IPreSptLoadMod
{
    private container: DependencyContainer;

    public preSptLoad(container: DependencyContainer): void 
    {
        this.container = container;

        const logger = container.resolve<ILogger>("WinstonLogger");
        const staticRouterModService = this.container.resolve<StaticRouterModService>("StaticRouterModService");

        staticRouterModService.registerStaticRouter(
            "ShowMeTheMoneyRoutes-GetRagfairConfigPriceRanges",
            [
				{
					url: "/showMeTheMoney/getRagfairConfigPriceRanges",

					action: (url, info, sessionId, output) => {
                        return new Promise((resolve) => {
                            const result = JSON.stringify(this.getRagfairConfigPriceRanges());
                            resolve(result);
                        });
					}
				}
            ],
            "Static-ShowMeTheMoneyRoutes"
        );

        staticRouterModService.registerStaticRouter(
            "ShowMeTheMoneyRoutes-GetPriceTable",
            [
				{
					url: "/showMeTheMoney/getPriceTable",

					action: (url, info, sessionId, output) => {
                        return new Promise((resolve) => {
                            const result = JSON.stringify(this.getPriceTable());
                            resolve(result);
                        });
					}
				}
            ],
            "Static-ShowMeTheMoneyRoutes"
        );

        logger.info("[Show Me The Money] Static routes hooked up. Ready to make some money...")
    }

    private getRagfairConfigPriceRanges() 
    {
        const configServer = this.container.resolve<ConfigServer>("ConfigServer");
        const ragfairConfig: IRagfairConfig = configServer.getConfig(ConfigTypes.RAGFAIR);

        return ragfairConfig.dynamic.priceRanges;
    }

    private getPriceTable() 
    {
        const databaseServer = this.container.resolve<DatabaseServer>("DatabaseServer");
        const priceTable = databaseServer.getTables().templates.prices;

        return priceTable;
    }
}

export const mod = new Mod();