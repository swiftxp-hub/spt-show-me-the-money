using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class FleaPriceService(DatabaseService databaseService, RagfairOfferHolder ragfairOfferHolder)
{
    public ConcurrentDictionary<MongoId, double> Get()
    {
        DatabaseTables databaseTables = databaseService.GetTables();

        HandbookBase handbookTable = databaseTables.Templates.Handbook;
        Dictionary<MongoId, TemplateItem> itemTable = databaseTables.Templates.Items;
        Dictionary<MongoId, double> priceTable = databaseTables.Templates.Prices;

        ConcurrentDictionary<MongoId, double> clonedPriceTable = [];
        List<MongoId> staleOfferIds = ragfairOfferHolder.GetStaleOfferIds();

        Parallel.ForEach(itemTable, item =>
        {
            if (item.Value.Properties?.CanSellOnRagfair == true)
            {
                double itemPrice = 0;
                if (!priceTable.TryGetValue(item.Key, out itemPrice))
                {
                    itemPrice = handbookTable.Items.SingleOrDefault(x => x.Id == item.Key)?.Price ?? 0;
                }

                IEnumerable<RagfairOffer>? offersOfType = ragfairOfferHolder.GetOffersByTemplate(item.Value.Id)?
                    .Where(x => x.RequirementsCost.HasValue
                        && (x.SellResults == null || x.SellResults?.Count == 0)
                        && !x.IsTraderOffer()
                        && !x.IsPlayerOffer()
                        && !staleOfferIds.Contains(x.Id));

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

                if (itemPrice > 0)
                    clonedPriceTable.TryAdd(item.Key, itemPrice);
            }
        });

        return clonedPriceTable;
    }
}