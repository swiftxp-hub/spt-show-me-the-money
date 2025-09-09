using BepInEx.Configuration;
using EFT.Communications;
using SwiftXP.ShowMeTheMoney.ConfigurationManager;
using SwiftXP.ShowMeTheMoney.Loggers;
using SwiftXP.ShowMeTheMoney.Models;

namespace SwiftXP.ShowMeTheMoney.Configuration;

public class PluginConfiguration
{
    public ConfigEntry<bool> EnablePlugin;

    public ConfigEntry<bool> ShowTraderPrices;

    public ConfigEntry<bool> ShowFleaPrices;

    public ConfigEntry<bool> IncludeFleaTax;

    public ConfigEntry<decimal> ToolTipDelay;

    public ConfigEntry<bool> ShowFleaTax;

    public PluginConfiguration(ConfigFile configFile)
    {
        configFile.SaveOnConfigSet = true;

        EnablePlugin = configFile.BindConfiguration("1. Main settings", "Enable plug-in", true, "Enable or disable the plug-in (Default: Enabled).", 3);
        ShowTraderPrices = configFile.BindConfiguration("1. Main settings", "Show trader price(s)", true, "Show the trader price(s) in the tool-tip (Default: Enabled).", 2);
        ShowFleaPrices = configFile.BindConfiguration("1. Main settings", "Show flea price(s)", true, "Show the flea price(s) in the tool-tip (Default: Enabled).", 1);
        ToolTipDelay = configFile.BindConfiguration("1. Main settings", "Tool-Tip delay", 0.0m, "Delays the tool-tip for x seconds. (Plug-In Default: 0, EFT Default: 0.6).", 0);
        
        IncludeFleaTax = configFile.BindConfiguration("2. Experimental settings", "Include flea tax", true, "Determines whether taxes for the flea market are included in the flea price. (Default: Enabled).", 1);
        ShowFleaTax = configFile.BindConfiguration("2. Experimental settings", "Show flea tax", false, "Show the flea tax in the tool-tip. (Default: Disabled).", 0);

        configFile.CreateButton(
            "3. Manual update",
            "Update flea prices",
            "Update now",
            "Pulls the current flea prices from your SPT server instance.",
            () =>
            {
                SimpleStaticLogger.Instance.Log(BepInEx.Logging.LogLevel.Info, "Updating flea prices...");
                bool pricesUpdated = RagfairPriceTable.Instance.UpdatePrices();

                GClass2314 updatedPricesMessage = new GClass2314(
                    "Flea prices updated successfully.",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Default
                );

                if (!pricesUpdated)
                {
                    updatedPricesMessage = new GClass2314(
                        "Flea prices could not be updated.",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Alert
                    );
                }

                NotificationManagerClass.DisplayNotification(updatedPricesMessage);
            },
            0
        );
    }
}