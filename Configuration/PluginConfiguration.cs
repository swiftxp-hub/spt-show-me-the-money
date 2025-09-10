using BepInEx.Configuration;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Models;
using SwiftXP.SPT.Common.Notifications;
using SwiftXP.SPT.ShowMeTheMoney.Enums;

namespace SwiftXP.SPT.ShowMeTheMoney.Configuration;

public class PluginConfiguration
{
    public ConfigEntry<bool> EnablePlugin;

    public ConfigEntry<bool> ShowTraderPrices;

    public ConfigEntry<bool> ShowFleaPrices;

    public ConfigEntry<CurrencyConversion> CurrencyConversionMode;

    public ConfigEntry<bool> RoublesOnly;

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
        
        CurrencyConversionMode = configFile.BindConfiguration("2. Currency conversion", "Currency conversion method", CurrencyConversion.Handbook,
            "Determines which source is used for currency conversion in the tooltip to determine the best trader offer. "
            + "'Handbook' is the value SPT defines in the handbook.json for each currency (by default $1 = ₽125, €1 = ₽133). "
            + "'Trader' takes the price you have to actually pay to get dollars/euros at Peacekeeper/Skier (by default $1 = ₽139, €1 = ₽153). "
            + "(Default: Handbook).", 0);

        RoublesOnly = configFile.BindConfiguration("2. Currency conversion", "Roubles only", false, "Only sale prices in roubles will be considered. Basically disables the currency conversion. (Default: Disabled).", 1);

        IncludeFleaTax = configFile.BindConfiguration("3. Experimental settings", "Include flea tax", true, "Determines whether taxes for the flea market are included in the flea price. (Default: Enabled).", 1);
        ShowFleaTax = configFile.BindConfiguration("3. Experimental settings", "Show flea tax", false, "Show the flea tax in the tool-tip. (Default: Disabled).", 0);

        configFile.CreateButton(
            "3. Manual update",
            "Update flea prices",
            "Update now",
            "Pulls the current flea prices from your SPT server instance.",
            () =>
            {
                Plugin.SimpleSptLogger.LogInfo("Updating flea prices...");
                bool pricesUpdated = RagfairPriceTableService.Instance.UpdatePrices();

                if (pricesUpdated)
                {
                    NotificationsService.Instance.SendLongNotice("Flea prices updated successfully.");

                }
                else
                {
                    NotificationsService.Instance.SendLongAlert("Flea prices could not be updated.");
                }
            },
            0
        );
    }
}