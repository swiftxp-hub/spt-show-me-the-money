import type { DependencyContainer } from "tsyringe";
import type { StaticRouterModService } from "@spt/services/mod/staticRouter/StaticRouterModService";
import type { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import { ILogger } from "@spt/models/spt/utils/ILogger";
import { FleaPricesService } from "./services/fleaPricesService";

class Mod implements IPreSptLoadMod
{
    private readonly modVersion = "1.9.0";

    private container?: DependencyContainer;
    private logger?: ILogger;

    public preSptLoad(container: DependencyContainer): void
    {
        this.container = container;
        this.container.register("FleaPricesService", FleaPricesService);

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

        this.logInfo("Static routes hooked up. Ready to make some money...");
    }

    private getFleaPrices(): Record<string, number>
    {
        const fleaPricesService = this.container!.resolve<FleaPricesService>("FleaPricesService");

        return fleaPricesService.get();
    }

    private logInfo(message: string): void
    {
        this.logger!.info(`[Show Me The Money v${this.modVersion}] ${message}`);
    }
}

export const mod = new Mod();