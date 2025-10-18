using EFT.InventoryLogic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public record TradeItem
{
    public TradeItem(Item item, TradePrice? traderPrice = null, TradePrice? fleaPrice = null)
    {
        XYCellSizeStruct itemSize = item.CalculateCellSize();
        this.ItemSlotCount = itemSize.X * itemSize.Y;

        this.Item = item;
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