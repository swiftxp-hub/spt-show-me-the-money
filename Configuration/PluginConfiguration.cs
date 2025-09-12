using BepInEx.Configuration;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Models;
using SwiftXP.SPT.Common.Notifications;
using SwiftXP.SPT.ShowMeTheMoney.Enums;
using UnityEngine;

namespace SwiftXP.SPT.ShowMeTheMoney.Configuration;

public class PluginConfiguration
{
    private ConfigEntry<bool> includeFleaTaxConfigEntry;

    private ConfigEntry<bool> showFleaTaxConfigEntry;

    public PluginConfiguration(ConfigFile configFile)
    {
        configFile.SaveOnConfigSet = true;

        this.EnablePlugin = configFile.BindConfiguration("1. Main settings", "Enable plug-in", true, "Enable or disable the plug-in (Default: Enabled).", 3);
        this.ShowTraderPrices = configFile.BindConfiguration("1. Main settings", "Show trader price(s)", true, "Show the trader price(s) in the tool-tip (Default: Enabled).", 2);
        this.ShowFleaPrices = configFile.BindConfiguration("1. Main settings", "Show flea price(s)", true, "Show the flea price(s) in the tool-tip (Default: Enabled).", 1);
        this.ToolTipDelay = configFile.BindConfiguration("1. Main settings", "Tool-Tip delay", 0.0m, "Delays the tool-tip for x seconds. (Plug-In Default: 0, EFT Default: 0.6).", 0);

        this.CurrencyConversionMode = configFile.BindConfiguration("2. Currency conversion", "Currency conversion method", CurrencyConversion.Handbook,
            "Determines which source is used for currency conversion in the tooltip to determine the best trader offer. "
            + "'Handbook' is the value SPT defines in the handbook.json for each currency (by default $1 = ₽125, €1 = ₽133). "
            + "'Trader' takes the price you have to actually pay to get dollars/euros at Peacekeeper/Skier (by default $1 = ₽139, €1 = ₽153) "
            + "(Default: Handbook).", 1);

        this.RoublesOnly = configFile.BindConfiguration("2. Currency conversion", "Roubles only", false, "Only sale prices in roubles will be considered. Basically disables the currency conversion (Default: Disabled).", 0);

        this.BestTradeColor = configFile.BindConfiguration("3. Appearance", "Best trade color", new Color(0.867f, 0.514f, 0.102f), "Defines the color used to highlight the best trade, trader or flea (Default: R 221, G 131, B 26).", 1);
        this.RenderInItalics = configFile.BindConfiguration("3. Appearance", "Italics", false, "Renders the price(s) in italics (Default: Disabled).", 0);

        this.EnableColorCoding = configFile.BindConfiguration("4. Color coding", "Enable color coding", true, "Uses color coding to give an quick and easy indicator how valueable an item is (Default: Enabled).", 13);

        this.PoorValue = configFile.BindConfiguration("4. Color coding", "Poor value (smaller than)", 2500m, "(Default: 2500).", 12);
        this.CommonValue = configFile.BindConfiguration("4. Color coding", "Common value (smaller than)", 20000m, "(Default: 20000).", 11);
        this.UncommonValue = configFile.BindConfiguration("4. Color coding", "Uncommon value (smaller than)", 40000m, "(Default: 40000).", 10);
        this.RareValue = configFile.BindConfiguration("4. Color coding", "Rare value (smaller than)", 70000m, "(Default: 70000).", 9);
        this.EpicValue = configFile.BindConfiguration("4. Color coding", "Epic value (smaller than) - (Legendary greater than or equal to)", 150000m, "(Default: 150000).", 8);

        configFile.CreateButton(
            "4. Color coding",
            "Set default values with only dealer prices in mind",
            "Set",
            "",
            () =>
            {
                this.PoorValue.Value = 5000m;
                this.CommonValue.Value = 7500m;
                this.UncommonValue.Value = 10000m;
                this.RareValue.Value = 15000m;
                this.EpicValue.Value = 20000m;
            },
            7
        );

        configFile.CreateButton(
            "4. Color coding",
            "Set default values with flea market prices in mind",
            "Set",
            "",
            () =>
            {
                this.PoorValue.Value = 2500m;
                this.CommonValue.Value = 20000m;
                this.UncommonValue.Value = 40000m;
                this.RareValue.Value = 70000m;
                this.EpicValue.Value = 150000m;
            },
            6
        );

        this.PoorColor = configFile.BindConfiguration("4. Color coding", "Poor color", new Color(0.62f, 0.62f, 0.62f), "(Default: R 157, G 157, B 157).", 5);
        this.CommonColor = configFile.BindConfiguration("4. Color coding", "Common color", new Color(1f, 1f, 1f), "(Default: R 255, G 255, B 255).", 4);
        this.UncommonColor = configFile.BindConfiguration("4. Color coding", "Uncommon color", new Color(0.12f, 1f, 0f), "(Default: R 30, G 255, B 0).", 3);
        this.RareColor = configFile.BindConfiguration("4. Color coding", "Rare color", new Color(0f, 0.44f, 0.87f), "(Default: R 0, G 112, B 221).", 2);
        this.EpicColor = configFile.BindConfiguration("4. Color coding", "Epic color", new Color(0.64f, 0.21f, 0.93f), "(Default: R 163, G 53, B 238).", 1);
        this.LegendaryColor = configFile.BindConfiguration("4. Color coding", "Legendary color", new Color(1f, 0.5f, 0f), "(Default: R 255, G 128, B 0).", 0);

        this.includeFleaTaxConfigEntry = configFile.BindConfiguration("5. Experimental settings", "Include flea tax", true, "Determines whether taxes for the flea market are included in the flea price (Default: Enabled).", 3);
        this.showFleaTaxConfigEntry = configFile.BindConfiguration("5. Experimental settings", "Show flea tax", false, "Show the flea tax in the tool-tip (Default: Disabled).", 2);
        this.FleaTaxToggleMode = configFile.BindConfiguration("5. Experimental settings", "Toggle-mode for flea tax", false, "When toggle mode is activated, the flea tax is only displayed when the specified key or key combination is pressed (Default: Disabled).", 1);
        this.FleaTaxToggleKey = configFile.BindConfiguration("5. Experimental settings", "Toggle-mode key", new KeyboardShortcut(KeyCode.LeftAlt), "Defines which key or key combination needs to be pressed to display the flea tax (Default: LeftAlt).", 0);

        this.FleaTaxToggleMode.SettingChanged += (_, _) =>
        {
            if (this.FleaTaxToggleMode.IsEnabled())
                Plugin.EnableTooltipUpdatePatch();
            else
                Plugin.DisableTooltipUpdatePatch();
        };

        configFile.CreateButton(
            "5. Manual update",
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

    #region Main settings 
    public ConfigEntry<bool> EnablePlugin { get; private set; }

    public ConfigEntry<bool> ShowTraderPrices { get; private set; }

    public ConfigEntry<bool> ShowFleaPrices { get; private set; }

    public ConfigEntry<decimal> ToolTipDelay { get; private set; }
    #endregion

    #region Currency conversion
    public ConfigEntry<CurrencyConversion> CurrencyConversionMode { get; private set; }

    public ConfigEntry<bool> RoublesOnly { get; private set; }
    #endregion

    #region Appearance
    public ConfigEntry<Color> BestTradeColor { get; private set; }

    public ConfigEntry<bool> RenderInItalics { get; private set; }
    #endregion

    #region Color coding
    public ConfigEntry<bool> EnableColorCoding { get; private set; }

    public ConfigEntry<decimal> PoorValue { get; private set; }

    public ConfigEntry<decimal> CommonValue { get; private set; }

    public ConfigEntry<decimal> UncommonValue { get; private set; }

    public ConfigEntry<decimal> RareValue { get; private set; }

    public ConfigEntry<decimal> EpicValue { get; private set; }

    public ConfigEntry<Color> PoorColor { get; private set; }

    public ConfigEntry<Color> CommonColor { get; private set; }

    public ConfigEntry<Color> UncommonColor { get; private set; }

    public ConfigEntry<Color> RareColor { get; private set; }

    public ConfigEntry<Color> EpicColor { get; private set; }

    public ConfigEntry<Color> LegendaryColor { get; private set; }
    #endregion

    #region Experimental settings
    public bool IncludeFleaTax
    {
        get
        {
            if (FleaTaxToggleMode.IsEnabled())
            {
                return this.includeFleaTaxConfigEntry.IsEnabled()
                    && this.FleaTaxToggleKey.GetValue().IsPressed();
            }

            return this.includeFleaTaxConfigEntry.IsEnabled();
        }
    }

    public bool ShowFleaTax
    {
        get
        {
            if (FleaTaxToggleMode.IsEnabled())
            {
                return this.showFleaTaxConfigEntry.IsEnabled()
                    && this.FleaTaxToggleKey.GetValue().IsPressed();
            }

            return this.showFleaTaxConfigEntry.IsEnabled();
        }
    }

    public ConfigEntry<bool> FleaTaxToggleMode { get; private set; }

    public ConfigEntry<KeyboardShortcut> FleaTaxToggleKey { get; private set; }
    #endregion
}