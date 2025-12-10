import { inject, injectable } from "tsyringe";
import { PartialRagfairConfig } from "../models/partialRagfairConfig";
import { ConfigServer } from "@spt/servers/ConfigServer"
import { ConfigTypes } from "@spt/models/enums/ConfigTypes";
import { IRagfairConfig } from "@spt/models/spt/config/IRagfairConfig"

@injectable()
export class RagfairConfigService
{
    constructor(@inject("ConfigServer") protected configServer: ConfigServer) { }

    public get(): PartialRagfairConfig
    {
        const ragfairConfig: IRagfairConfig = this.configServer.getConfig(ConfigTypes.RAGFAIR);
        const partialRagfairConfig: PartialRagfairConfig = {
            itemPriceMultiplier: ragfairConfig.dynamic.itemPriceMultiplier,

            base: ragfairConfig.sell.chance.base,
            maxSellChancePercent: ragfairConfig.sell.chance.maxSellChancePercent,
            sellMultiplier: ragfairConfig.sell.chance.sellMultiplier
        };

        return partialRagfairConfig;
    }
}