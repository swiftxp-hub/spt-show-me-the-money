import type { DependencyContainer } from "tsyringe";
import type { StaticRouterModService } from "@spt/services/mod/staticRouter/StaticRouterModService";
import type { ConfigServer } from "@spt/servers/ConfigServer";
import { ConfigTypes } from "@spt/models/enums/ConfigTypes"
import { IRagfairConfig } from "@spt/models/spt/config/IRagfairConfig";
import type { DatabaseServer } from "@spt/servers/DatabaseServer";
import type { DatabaseService } from "@spt/services/DatabaseService";
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
            "ShowMeTheMoneyRoutes-GetCurrencyPurchasePrices",
            [
				{
					url: "/showMeTheMoney/getCurrencyPurchasePrices",

					action: (url, info, sessionId, output) => {
                        return new Promise((resolve) => {
                            const result = JSON.stringify(this.getCurrencyPurchasePrices());
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

        logger.info("[Show Me The Money] Static routes hooked up. Ready to make some money...")
    }

    private getCurrencyPurchasePrices()
    {
        const databaseService = this.container.resolve<DatabaseService>("DatabaseService");
        const peacekeeper = databaseService.getTrader("5935c25fb3acc3127c3d8cd9");
        const skier = databaseService.getTrader("58330581ace78e27b8b10cee");

        const eurPrice = skier.assort.barter_scheme["677536ee7949f87882036fb0"][0][0].count;
        const usdPrice = peacekeeper.assort.barter_scheme["676d24a5798491c5260f4b01"][0][0].count;

        return { eur: eurPrice, usd: usdPrice };
    }

    private getPriceTable() 
    {
        const databaseServer = this.container.resolve<DatabaseServer>("DatabaseServer");
        const priceTable = databaseServer.getTables().templates.prices;

        return priceTable;
    }

    private getRagfairConfigPriceRanges() 
    {
        const configServer = this.container.resolve<ConfigServer>("ConfigServer");
        const ragfairConfig: IRagfairConfig = configServer.getConfig(ConfigTypes.RAGFAIR);

        return ragfairConfig.dynamic.priceRanges;
    }
}

export const mod = new Mod();