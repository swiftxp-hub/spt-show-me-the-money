import { inject, injectable } from "tsyringe";
import { ItemHelper } from "@spt/helpers/ItemHelper";
import { RagfairPriceService } from "@spt/services/RagfairPriceService";
import { RagfairOfferService } from "@spt/services/RagfairOfferService";
import { PaymentHelper } from "@spt/helpers/PaymentHelper";
import { IRagfairOffer, IOfferRequirement } from "@spt/models/eft/ragfair/IRagfairOffer"
import { MemberCategory } from "@spt/models/enums/MemberCategory"

@injectable()
export class FleaPricesService
{
    constructor(
        @inject("ItemHelper") protected itemHelper: ItemHelper,
        @inject("RagfairPriceService") protected ragfairPriceService: RagfairPriceService,
        @inject("RagfairOfferService") protected ragfairOfferService: RagfairOfferService,
        @inject("PaymentHelper") protected paymentHelper: PaymentHelper
    ) { }

    public get(): Record<string, number>
    {
        const fleaPrices: Record<string, number> = this.ragfairPriceService.getAllFleaPrices();
        let result: Record<string, number> = { ...fleaPrices };

        for (const [key, value] of Object.entries(fleaPrices))
        {
            try
            {
                const newPrice = this.getAveragePriceFromOffers(key);
                if (newPrice > 0)
                {
                    result[key] = newPrice;
                }
            }
            catch (_e) { }
        }

        return result;
    }

    private getAveragePriceFromOffers(itemTemplateId: string): number
    {
        try
        {
            const offers = this.ragfairOfferService.getOffersOfType(itemTemplateId);
            if (offers != null && offers.length > 0)
            {
                const countableOffers = offers.filter((x: IRagfairOffer) =>
                {
                    return !x.requirements.some((req: IOfferRequirement) => !this.paymentHelper.isMoneyTpl(req._tpl))
                        && x.user.memberType != MemberCategory.TRADER;
                });

                if (countableOffers.length > 0)
                {
                    let offerSum: number = 0;
                    let countedOffers: number = 0;

                    for (const ragfairOffer of countableOffers)
                    {
                        const firstItem = ragfairOffer.items[0];
                        const itemCount = (ragfairOffer.sellInOnePiece ?? false)
                            ? firstItem.upd?.StackObjectsCount
                            ?? 1 : 1;

                        const perItemPrice = ragfairOffer.requirementsCost / itemCount;
                        if (perItemPrice > 0 && perItemPrice > 0)
                        {
                            offerSum += perItemPrice;
                            ++countedOffers;
                        }
                    }

                    if (offerSum > 0)
                        return Math.round(offerSum / countedOffers);
                }
            }
        }
        catch (_e) { }

        return 0;
    }
}