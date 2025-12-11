import type { DependencyContainer } from "tsyringe";
import type { StaticRouterModService } from "@spt/services/mod/staticRouter/StaticRouterModService";
import type { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import { ILogger } from "@spt/models/spt/utils/ILogger";
import { FleaPricesService } from "./services/fleaPricesService";
import { RagfairConfigService } from "./services/ragfairConfigService";
import { PartialRagfairConfig } from "./models/partialRagfairConfig";

class Mod implements IPreSptLoadMod
{
    private readonly modVersion = "1.9.0";

    private container?: DependencyContainer;
    private logger?: ILogger;

    public preSptLoad(container: DependencyContainer): void
    {
        this.container = container;
        this.container.register("FleaPricesService", FleaPricesService);
        this.container.register("RagfairConfigService", RagfairConfigService);

        this.logger = container.resolve<ILogger>("WinstonLogger");

        const staticRouterModService = this.container.resolve<StaticRouterModService>("StaticRouterModService");

        staticRouterModService.registerStaticRouter(
            "ShowMeTheMoneyRoutes-GetFleaPrices",
            [
                {
                    url: "/showMeTheMoney/getFleaPrices",

                    action: (url, info, sessionId, output) =>
                    {
                        return new Promise((resolve) =>
                        {
                            const result = JSON.stringify(this.getFleaPrices());
                            resolve(result);
                        });
                    }
                }
            ],
            "Static-ShowMeTheMoneyRoutes"
        );

        staticRouterModService.registerStaticRouter(
            "ShowMeTheMoneyRoutes-GetPartialRagfairConfig",
            [
                {
                    url: "/showMeTheMoney/getPartialRagfairConfig",

                    action: (url, info, sessionId, output) =>
                    {
                        return new Promise((resolve) =>
                        {
                            const result = JSON.stringify(this.getPartialRagfairConfig());
                            resolve(result);
                        });
                    }
                }
            ],
            "Static-ShowMeTheMoneyRoutes"
        );

        this.logInfo("Static routes hooked up. Ready to make some money...");
    }

    private getFleaPrices(): Record<string, number>
    {
        const fleaPricesService = this.container!.resolve<FleaPricesService>("FleaPricesService");

        return fleaPricesService.get();
    }

    private getPartialRagfairConfig(): PartialRagfairConfig
    {
        const ragfairConfigService = this.container!.resolve<RagfairConfigService>("RagfairConfigService");

        return ragfairConfigService.get();
    }

    private logInfo(message: string): void
    {
        this.logger!.info(`[Show Me The Money v${this.modVersion}] ${message}`);
    }
}

export const mod = new Mod();