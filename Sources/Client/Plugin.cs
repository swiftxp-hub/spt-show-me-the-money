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
[BepInDependency("com.SPT.custom", "4.0.11")]
[BepInProcess("EscapeFromTarkov.exe")]
public class Plugin : BaseUnityPlugin
{
    private static ModulePatch? s_tooltipUpdatePatch;

    public static void EnableTooltipUpdatePatch()
    {
        s_tooltipUpdatePatch!.Enable();
    }

    public static void DisableTooltipUpdatePatch()
    {
        s_tooltipUpdatePatch!.Disable();
    }

    private void Awake()
    {
        InitLogger();
        BindBepInExConfiguration();
        InitPriceServices();
        EnablePatches();
    }

    private static void InitLogger()
    {
        SptLogger = new SimpleSptLogger(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION);
    }

    private void BindBepInExConfiguration()
    {
        SptLogger!.LogInfo("Bind configuration...");

        Configuration = new PluginConfiguration(Config);
    }

    private void InitPriceServices()
    {
        SptLogger!.LogInfo("Initializing config and flea price service...");

        StartCoroutine(PartialRagfairConfigService.Instance.GetPartialRagfairConfig());
        StartCoroutine(FleaPricesService.Instance.UpdatePrices());
    }

    private static void EnablePatches()
    {
        SptLogger!.LogInfo("Enable patches...");

        new TraderClassPatch().Enable();

        new EditBuildScreenShowPatch().Enable();
        new EditBuildScreenClosePatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();

        new InventoryScreenClosePatch().Enable();

        new SimpleTooltipShowPatch().Enable();

        s_tooltipUpdatePatch = new TooltipUpdatePatch();

        if (Configuration!.FleaTaxToggleMode.IsEnabled())
            EnableTooltipUpdatePatch();
    }

    public static PluginConfiguration? Configuration { get; set; }

    public static SimpleSptLogger? SptLogger { get; set; }

    public static Item? HoveredItem { get; set; }

    public static bool DisableTemporary { get; set; }
}