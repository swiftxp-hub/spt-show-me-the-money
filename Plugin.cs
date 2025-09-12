using BepInEx;
using EFT.InventoryLogic;
using SwiftXP.SPT.ShowMeTheMoney.Configuration;
using SwiftXP.SPT.ShowMeTheMoney.Models;
using SwiftXP.SPT.ShowMeTheMoney.Patches;
using SwiftXP.SPT.Common.Loggers;
using SPT.Reflection.Patching;
using SwiftXP.SPT.Common.ConfigurationManager;

namespace SwiftXP.SPT.ShowMeTheMoney;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("EscapeFromTarkov.exe")]
public class Plugin : BaseUnityPlugin
{
    private static ModulePatch? TooltipUpdatePatch;

    public static void EnableTooltipUpdatePatch()
    {
        TooltipUpdatePatch!.Enable();
    }

    public static void DisableTooltipUpdatePatch()
    {
        TooltipUpdatePatch!.Disable();
    }

    private void Awake()
    {
        BindBepInExConfiguration();
        InitPriceRangesAndTable();
        EnablePatches();
    }

    private void BindBepInExConfiguration()
    {
        SimpleSptLogger.LogInfo("Bind configuration...");

        Configuration = new PluginConfiguration(Config);
    }

    private void InitPriceRangesAndTable()
    {
        SimpleSptLogger.LogInfo("Initializing currency purchase prices, price ranges and price table...");

        CurrencyPurchasePricesService.Instance.GetCurrencyPurchasePrices();
        RagfairPriceRangesService.Instance.GetPriceRanges();
        RagfairPriceTableService.Instance.UpdatePrices();
    }

    private void EnablePatches()
    {
        SimpleSptLogger.LogInfo("Enable patches...");

        new TraderClassPatch().Enable();

        new EditBuildScreenShowPatch().Enable();
        new EditBuildScreenClosePatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();
        new SimpleTooltipShowPatch().Enable();

        TooltipUpdatePatch = new TooltipUpdatePatch();

        if (Configuration!.FleaTaxToggleMode.IsEnabled())
            EnableTooltipUpdatePatch();
    }

    public static SimpleSptLogger SimpleSptLogger = new(MyPluginInfo.PLUGIN_GUID);

    public static PluginConfiguration? Configuration;

    public static Item? HoveredItem { get; set; }

    public static bool DisableTemporary { get; set; }
}