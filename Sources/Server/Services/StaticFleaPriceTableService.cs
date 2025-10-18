using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Server.Services;

[Injectable(InjectionType.Scoped)]
public class StaticFleaPriceTableService(DatabaseService databaseService)
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

                if (itemPrice.HasValue)
                    clonedPriceTable.TryAdd(item.Key, itemPrice.Value);
            }
        });

        return clonedPriceTable;
    }
}