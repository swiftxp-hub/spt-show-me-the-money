using EFT.InventoryLogic;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class TradeItem
{
    public TradeItem(Item item, int itemSlotCount, TradePrice? traderPrice = null, TradePrice? fleaPrice = null)
    {
        Item = item;
        ItemSlotCount = itemSlotCount;
        TraderPrice = traderPrice;
        FleaPrice = fleaPrice;
    }

    public Item Item { get; }

    public int ItemSlotCount { get; }

    public int ItemObjectCount
    {
        get
        {
            return Item.StackObjectsCount;
        }
    }

    public TradePrice? TraderPrice { get; set; }

    public TradePrice? FleaPrice { get; set; }
}