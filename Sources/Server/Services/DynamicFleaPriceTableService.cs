using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class DynamicFleaPriceTableService(DatabaseService databaseService, RagfairOfferService fleaOfferService)
{
    public ConcurrentDictionary<MongoId, double> Get()
    {
        HandbookBase handbookTable = databaseService.GetTables().Templates.Handbook;
        Dictionary<MongoId, TemplateItem> itemTable = databaseService.GetTables().Templates.Items;
        Dictionary<MongoId, double> priceTable = databaseService.GetTables().Templates.Prices;

        ConcurrentDictionary<MongoId, double> clonedPriceTable = [];
        Parallel.ForEach(itemTable, item =>
        {
            if (item.Value.Properties?.CanSellOnRagfair == true)
            {
                double? itemPrice = null;
                if (priceTable.TryGetValue(item.Key, out double price))
                {
                    itemPrice = price;
                }
                else
                {
                    itemPrice = handbookTable.Items.FirstOrDefault(x => x.Id == item.Key)?.Price;
                }

                IEnumerable<RagfairOffer>? offersOfType = fleaOfferService.GetOffersOfType(item.Value.Id)?
                    .Where(x => x.RequirementsCost.HasValue
                        && (x.SellResults == null || x.SellResults?.Count == 0)
                        && x.User?.MemberType != MemberCategory.Trader);

                if (offersOfType != null && offersOfType.Any())
                {
                    double averageOffersPrice = 0;
                    int countedOffers = 0;

                    foreach (RagfairOffer FleaOffer in offersOfType)
                    {
                        averageOffersPrice += FleaOffer.RequirementsCost!.Value;
                        ++countedOffers;
                    }

                    if (averageOffersPrice > 0)
                    {
                        itemPrice = averageOffersPrice / countedOffers;
                    }
                }

                if (itemPrice.HasValue)
                    clonedPriceTable.TryAdd(item.Key, itemPrice.Value);
            }
        });

        return clonedPriceTable;
    }
}