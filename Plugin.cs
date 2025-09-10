using BepInEx;
using EFT.InventoryLogic;
using SwiftXP.SPT.ShowMeTheMoney.Configuration;
using SwiftXP.SPT.ShowMeTheMoney.Models;
using SwiftXP.SPT.ShowMeTheMoney.Patches;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.SPT.ShowMeTheMoney;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("EscapeFromTarkov.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string RemotePathToGetCurrencyPurchasePrices = "/showMeTheMoney/getCurrencyPurchasePrices";

    public const string RemotePathToGetPriceTable = "/showMeTheMoney/getPriceTable";

    public const string RemotePathToGetRagfairConfigPriceRanges = "/showMeTheMoney/getRagfairConfigPriceRanges";

    public const string HighlightColorCode = "#dd831a";

    public static SimpleSptLogger SimpleSptLogger = new(MyPluginInfo.PLUGIN_GUID);

    public static PluginConfiguration? Configuration;

    public static Item? HoveredItem { get; set; }

    public static bool DisableTemporary { get; set; }

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

        new TraderPatch().Enable();

        new EditBuildScreenShowPatch().Enable();
        new EditBuildScreenClosePatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();
        new SimpleTooltipPatch().Enable();
    }
}