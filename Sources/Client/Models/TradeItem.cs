using EFT.InventoryLogic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public class TradeItem
{
    public TradeItem(Item item, TradePrice? traderPrice = null, TradePrice? fleaPrice = null)
    {
        XYCellSizeStruct itemSize = item.CalculateCellSize();
        ItemSlotCount = itemSize.X * itemSize.Y;

        Item = item;
        TraderPrice = traderPrice;
        FleaPrice = fleaPrice;
    }

    public TradePrice? FleaPrice { get; set; }

    public Item Item { get; }

    public int ItemObjectCount
    {
        get
        {
            return Item.StackObjectsCount;
        }
    }

    public int ItemSlotCount { get; }

    public TradePrice? TraderPrice { get; set; }
}