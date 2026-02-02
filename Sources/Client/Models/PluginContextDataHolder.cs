using EFT.InventoryLogic;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Models;

public static class PluginContextDataHolder
{
    private static volatile PluginContextData s_currentData = new(null, null, null, false);

    public static PluginContextData Current
    {
        get { return s_currentData; }
    }

    public static void SetDisableTemporary(bool disableTemporary)
    {
        PluginContextData pluginContextData = new(Current.SptLogger, Current.Configuration, Current.HoveredItem, disableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }

    public static void SetHoveredItem(Item? hoveredItem)
    {
        PluginContextData pluginContextData = new(Current.SptLogger, Current.Configuration, hoveredItem, Current.DisableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }

    public static void SetContextInstances(SimpleSptLogger simpleSptLogger, PluginConfiguration pluginConfiguration)
    {
        PluginContextData pluginContextData = new(simpleSptLogger, pluginConfiguration, Current.HoveredItem, Current.DisableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }
}