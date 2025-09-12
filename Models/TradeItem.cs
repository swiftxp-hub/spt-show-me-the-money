using EFT.InventoryLogic;

namespace SwiftXP.SPT.ShowMeTheMoney.Models;

public class TradeItem
{
    public TradeItem(Item item, int itemSlotCount, TradePrice? traderPrice = null, TradePrice? fleaPrice = null)
    {
        this.Item = item;
        this.ItemSlotCount = itemSlotCount;
        this.TraderPrice = traderPrice;
        this.FleaPrice = fleaPrice;
    }

    public TradePrice? FleaPrice { get; set; }

    public Item Item { get; }

    public int ItemObjectCount
    {
        get
        {
            return this.Item.StackObjectsCount;
        }
    }

    public int ItemSlotCount { get; }

    public TradePrice? TraderPrice { get; set; }
}