using EFT.InventoryLogic;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Contexts.Holders;

public static class PluginContextHolder
{
    private static volatile PluginContext s_currentData = new(null, null, null, false);

    public static PluginContext Current
    {
        get { return s_currentData; }
    }

    public static void SetDisableTemporary(bool disableTemporary)
    {
        PluginContext pluginContextData = new(Current.SptLogger, Current.Configuration, Current.HoveredItem, disableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }

    public static void SetHoveredItem(Item? hoveredItem)
    {
        PluginContext pluginContextData = new(Current.SptLogger, Current.Configuration, hoveredItem, Current.DisableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }

    public static void SetContextInstances(SimpleSptLogger simpleSptLogger, PluginConfiguration pluginConfiguration)
    {
        PluginContext pluginContextData = new(simpleSptLogger, pluginConfiguration, Current.HoveredItem, Current.DisableTemporary);
        System.Threading.Interlocked.Exchange(ref s_currentData, pluginContextData);
    }
}