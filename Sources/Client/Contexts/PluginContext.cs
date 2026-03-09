using EFT.InventoryLogic;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Contexts;

public record PluginContext
{
    public PluginContext(SimpleSptLogger? simpleSptLogger, PluginConfiguration? pluginConfiguration,
        Item? hoveredItem, bool disableTemporary)
    {
        SptLogger = simpleSptLogger;
        Configuration = pluginConfiguration;
        HoveredItem = hoveredItem;
        DisableTemporary = disableTemporary;
    }

    public SimpleSptLogger? SptLogger { get; }

    public PluginConfiguration? Configuration { get; }

    public Item? HoveredItem { get; }

    public bool DisableTemporary { get; }
}