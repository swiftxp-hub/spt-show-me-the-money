using BepInEx;
using EFT.InventoryLogic;
using SwiftXP.ShowMeTheMoney.Configuration;
using SwiftXP.ShowMeTheMoney.Loggers;
using SwiftXP.ShowMeTheMoney.Models;
using SwiftXP.ShowMeTheMoney.Patches;

namespace SwiftXP.ShowMeTheMoney;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("EscapeFromTarkov.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string RemotePathToGetRagfairConfigPriceRanges = "/showMeTheMoney/getRagfairConfigPriceRanges";

    public const string RemotePathToGetPriceTable = "/showMeTheMoney/getPriceTable";

    public const string HighlightColorCode = "#dd831a";

    public static PluginConfiguration? Configuration;

    public static Item? HoveredItem { get; set; }

    private void Awake()
    {
        BindBepInExConfiguration();
        InitPriceRangesAndTable();
        EnablePatches();
    }

    private void BindBepInExConfiguration()
    {
        SimpleStaticLogger.Instance.Log(BepInEx.Logging.LogLevel.Info, "Bind configuration...");

        Configuration = new PluginConfiguration(Config);
    }

    private void InitPriceRangesAndTable()
    {
        SimpleStaticLogger.Instance.Log(BepInEx.Logging.LogLevel.Info, "Initializing price ranges and table...");

        RagfairPriceRanges.Instance.GetPriceRanges();
        RagfairPriceTable.Instance.UpdatePrices();
    }

    private void EnablePatches()
    {
        SimpleStaticLogger.Instance.Log(BepInEx.Logging.LogLevel.Info, "Enable patches...");

        new TraderPatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();
        new SimpleTooltipPatch().Enable();
    }
}