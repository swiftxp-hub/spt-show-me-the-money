using BepInEx;
using EFT.InventoryLogic;
using SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;
using SwiftXP.SPT.ShowMeTheMoney.Client.Patches;
using SwiftXP.SPT.Common.Loggers;
using SPT.Reflection.Patching;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Client.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Client;

[BepInPlugin("com.swiftxp.spt.showmethemoney", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
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
        InitLogger();
        BindBepInExConfiguration();
        InitPriceServices();
        EnablePatches();
    }

    private void InitLogger()
    {
        SimpleSptLogger.Instance.Init(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION);
    }

    private void BindBepInExConfiguration()
    {
        SimpleSptLogger.Instance.LogInfo("Bind configuration...");

        Configuration = new PluginConfiguration(Config);
    }

    private void InitPriceServices()
    {
        SimpleSptLogger.Instance.LogInfo("Initializing price services...");

        CurrencyPurchasePricesService.Instance.GetCurrencyPurchasePrices();
        RagfairPriceTableService.Instance.UpdatePrices();
    }

    private void EnablePatches()
    {
        SimpleSptLogger.Instance.LogInfo("Enable patches...");

        new TraderClassPatch().Enable();

        new EditBuildScreenShowPatch().Enable();
        new EditBuildScreenClosePatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();
        new SimpleTooltipShowPatch().Enable();

        new InventoryScreenShowPatch().Enable();

        TooltipUpdatePatch = new TooltipUpdatePatch();

        if (Configuration!.FleaTaxToggleMode.IsEnabled())
            EnableTooltipUpdatePatch();
    }

    public static PluginConfiguration? Configuration;

    public static Item? HoveredItem { get; set; }

    public static bool DisableTemporary { get; set; }
}