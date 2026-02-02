using BepInEx.Configuration;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.Notifications;
using UnityEngine;
using System;
using SwiftXP.SPT.ShowMeTheMoney.Client.Enums;
using SwiftXP.SPT.ShowMeTheMoney.Client.Services;
using System.Linq;
using System.Collections.Generic;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;

public class PluginConfiguration
{
    private ConfigEntry<bool> _includeFleaTaxConfigEntry;

    private ConfigEntry<bool> _showFleaTaxConfigEntry;

    public PluginConfiguration(ConfigFile configFile, Action onForceUpdate)
    {
        // --- 1. Main settings
        EnablePlugin = configFile.BindConfiguration("1. Main settings", "Enable plug-in", true, $"Enable or disable the plug-in.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 7);
        EnableTraderPrices = configFile.BindConfiguration("1. Main settings", "Enable trader price(s)", true, $"Enable the trader price(s) in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 6);
        EnableFleaPrices = configFile.BindConfiguration("1. Main settings", "Enable flea price(s)", true, $"Enable the flea price(s) in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 5);
        ShowPricePerSlot = configFile.BindConfiguration("1. Main settings", "Show price-per-slot", true, $"Show the price-per-slot in the tool-tip. The mod continues to calculate in the background using price-per-slot, even if the display is deactivated.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 4);
        ShowWeaponModsPrice = configFile.BindConfiguration("1. Main settings", "Show weapon-mods price", true, $"Show the total price of all modifications installed in a weapon.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 3);
        ShowArmorPlatesPrice = configFile.BindConfiguration("1. Main settings", "Show armor-plates price", true, $"Show the total price of all (removable-)plates installed in an armor.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 2);

        TradersToIgnore = configFile.BindConfiguration("1. Main settings", "Trader(s) to ignore", "", $"List of traders to be ignored, separated by commas. You can enter either the trader's ID or their name.{Environment.NewLine}{Environment.NewLine}Example: Prapor,Skier,Peacekeeper", 1);

        ToolTipDelay = configFile.BindConfiguration("1. Main settings", "Tool-Tip delay", 0.0m, $"Delays the tool-tip for x seconds.{Environment.NewLine}{Environment.NewLine}(Plug-In Default: 0, EFT Default: 0.6)", 0);

        // --- 2. Currency conversion
        RoublesOnly = configFile.BindConfiguration("2. Currency conversion", "Roubles only", false, $"Only sales prices in roubles will be considered. Basically no longer displays trades from traders who do not buy in rubles.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 0);

        // --- 3. Appearance
        BestTradeColor = configFile.BindConfiguration("3. Appearance", "Best trade color", new Color(1.000f, 1.000f, 1.000f), $"Defines the color used to highlight the best trade, trader or flea.{Environment.NewLine}{Environment.NewLine}(Default: R 255, G 255, B 255)", 2);
        FontSize = configFile.BindConfiguration("3. Appearance", "Font size", TooltipFontSize.Smaller, $"Changes the font size of the price(s).{Environment.NewLine}{Environment.NewLine}(Default: Smaller)", 1);
        RenderInItalics = configFile.BindConfiguration("3. Appearance", "Italics", false, $"Renders the price(s) in italics.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 0);

        // --- 4. Color coding
        EnableColorCoding = configFile.BindConfiguration("4. Color coding", "Enable color coding (based on price-per-slot)", true, $"Uses color coding to give an quick and easy indicator how valueable an item is. Always based on price-per-slot, except for ammunition, if you activate the respective feature. Default colors are from WoW.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 18);
        ColorCodingMode = configFile.BindConfiguration("4. Color coding", "Color coding mode", Enums.ColorCodingMode.Both, $"Defines the color coding mode.{Environment.NewLine}{Environment.NewLine}(Default: Both)", 17);

        PoorValue = configFile.BindConfiguration("4. Color coding", "Poor value (smaller than)", 900m, "(Default: 900)", 16);
        CommonValue = configFile.BindConfiguration("4. Color coding", "Common value (smaller than)", 12000m, "(Default: 12000)", 15);
        UncommonValue = configFile.BindConfiguration("4. Color coding", "Uncommon value (smaller than)", 21000m, "(Default: 21000)", 14);
        RareValue = configFile.BindConfiguration("4. Color coding", "Rare value (smaller than)", 38000m, "(Default: 38000)", 13);
        EpicValue = configFile.BindConfiguration("4. Color coding", "Epic value (smaller than) - everything above that is considered legendary", 92000m, "(Default: 92000)", 12);

        UseCaliberPenetrationPower = configFile.BindConfiguration("4. Color coding", "Use penetration power instead of price value for color coding of ammunition", true, $"Uses the caliber penetration power value instead of the price value for color coding of ammunition.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 11);

        PoorPenetrationValue = configFile.BindConfiguration("4. Color coding", "Poor penetration value (smaller than)", 15m, "(Default: 15)", 10);
        CommonPenetrationValue = configFile.BindConfiguration("4. Color coding", "Common penetration value (smaller than)", 25m, "(Default: 25)", 9);
        UncommonPenetrationValue = configFile.BindConfiguration("4. Color coding", "Uncommon penetration value (smaller than)", 34m, "(Default: 34)", 8);
        RarePenetrationValue = configFile.BindConfiguration("4. Color coding", "Rare penetration value (smaller than)", 43m, "(Default: 43)", 7);
        EpicPenetrationValue = configFile.BindConfiguration("4. Color coding", "Epic penetration value (smaller than) - everything above that is considered legendary", 55m, "(Default: 55)", 6);

        PoorColor = configFile.BindConfiguration("4. Color coding", "Poor color", new Color(0.62f, 0.62f, 0.62f), "(Default: R 157, G 157, B 157)", 5);
        CommonColor = configFile.BindConfiguration("4. Color coding", "Common color", new Color(1f, 1f, 1f), "(Default: R 255, G 255, B 255)", 4);
        UncommonColor = configFile.BindConfiguration("4. Color coding", "Uncommon color", new Color(0.12f, 1f, 0f), "(Default: R 30, G 255, B 0)", 3);
        RareColor = configFile.BindConfiguration("4. Color coding", "Rare color", new Color(0f, 0.44f, 0.87f), "(Default: R 0, G 112, B 221)", 2);
        EpicColor = configFile.BindConfiguration("4. Color coding", "Epic color", new Color(0.64f, 0.21f, 0.93f), "(Default: R 163, G 53, B 238)", 1);
        LegendaryColor = configFile.BindConfiguration("4. Color coding", "Legendary color", new Color(1f, 0.5f, 0f), "(Default: R 255, G 128, B 0)", 0);

        // --- 5. Flea market
        AlwaysShowFleaPrice = configFile.BindConfiguration("5. Flea market", "Always show flea price", false, $"Always show the flea price of an item even if the flea market is not yet unlocked or the item is not found-in-raid and the \"Can only sell items with 'Found in raid' tag\" setting in SPT/SVM is enabled.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 8);
        FleaPriceMultiplicand = configFile.BindConfiguration("5. Flea market", "Flea price multiplicand", 1.0m, $"Sets the multiplicand by which the average flea market price is multiplied and then displayed in the tooltip. The following calculation is performed:{Environment.NewLine}{Environment.NewLine}Average flea market price of the item * Flea price multiplicand{Environment.NewLine}{Environment.NewLine}(Default: 1.0)", 7);
        _includeFleaTaxConfigEntry = configFile.BindConfiguration("5. Flea market", "Include flea tax", false, $"Determines whether taxes for the flea market are included in the flea price.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 6);
        _showFleaTaxConfigEntry = configFile.BindConfiguration("5. Flea market", "Show flea tax", false, $"Show the flea tax in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 5);
        FleaTaxToggleMode = configFile.BindConfiguration("5. Flea market", "Toggle-mode for flea tax", false, $"When toggle mode is activated, the flea tax is only displayed when the specified key or key combination is pressed.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 4);
        FleaTaxToggleKey = configFile.BindConfiguration("5. Flea market", "Toggle-mode key", new KeyboardShortcut(KeyCode.LeftAlt), $"Defines which key or key combination needs to be pressed to display the flea tax.{Environment.NewLine}{Environment.NewLine}(Default: LeftAlt)", 3);

        UpdateInterval = configFile.BindConfiguration("5. Flea market", "Update flea price interval", 5, $"Specifies the interval in minutes at which the mod updates flea-market prices. More frequent updates can provide more accurate prices but may also increase server load.{Environment.NewLine}{Environment.NewLine}(Default: 5 minutes)", 2);
        UpdateDuringRaid = configFile.BindConfiguration("5. Flea market", "Update flea price during raids", false, $"Specifies whether flea-market prices are updated while in-raid.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 1);

        configFile.CreateButton(
            "5. Flea market",
            "Update flea prices",
            "Update now",
            "Pulls the current flea prices from your SPT server instance (can take up to 30 seconds).",
            () =>
            {
                onForceUpdate();
                EftNotificationHelper.SendLongNotice("Flea prices will update as soon as possible...");
            },
            0
        );

        TradersToIgnore.SettingChanged += (_, _) =>
        {
            List<string> tradersToIngore = [.. (TradersToIgnore.GetValue() ?? string.Empty)
                .Trim()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)];

            TraderPriceService.Instance.TradersToIgnore = tradersToIngore;
        };

        FleaTaxToggleMode.SettingChanged += (_, _) =>
        {
            if (FleaTaxToggleMode.IsEnabled())
                Plugin.EnableTooltipUpdatePatch();
            else
                Plugin.DisableTooltipUpdatePatch();
        };

        configFile.SaveOnConfigSet = true;
    }

    #region Main settings 
    public ConfigEntry<bool> EnablePlugin { get; set; }

    public ConfigEntry<bool> EnableTraderPrices { get; set; }

    public ConfigEntry<bool> EnableFleaPrices { get; set; }

    public ConfigEntry<bool> ShowPricePerSlot { get; set; }

    public ConfigEntry<bool> ShowWeaponModsPrice { get; set; }

    public ConfigEntry<bool> ShowArmorPlatesPrice { get; set; }

    public ConfigEntry<string> TradersToIgnore { get; set; }

    public ConfigEntry<decimal> ToolTipDelay { get; set; }
    #endregion

    #region Currency conversion
    public ConfigEntry<bool> RoublesOnly { get; set; }
    #endregion

    #region Appearance
    public ConfigEntry<Color> BestTradeColor { get; set; }

    public ConfigEntry<TooltipFontSize> FontSize { get; set; }

    public ConfigEntry<bool> RenderInItalics { get; set; }
    #endregion

    #region Color coding
    public ConfigEntry<bool> EnableColorCoding { get; set; }

    public ConfigEntry<ColorCodingMode> ColorCodingMode { get; set; }

    public ConfigEntry<decimal> PoorValue { get; set; }

    public ConfigEntry<decimal> CommonValue { get; set; }

    public ConfigEntry<decimal> UncommonValue { get; set; }

    public ConfigEntry<decimal> RareValue { get; set; }

    public ConfigEntry<decimal> EpicValue { get; set; }

    public ConfigEntry<bool> UseCaliberPenetrationPower { get; set; }

    public ConfigEntry<decimal> PoorPenetrationValue { get; set; }

    public ConfigEntry<decimal> CommonPenetrationValue { get; set; }

    public ConfigEntry<decimal> UncommonPenetrationValue { get; set; }

    public ConfigEntry<decimal> RarePenetrationValue { get; set; }

    public ConfigEntry<decimal> EpicPenetrationValue { get; set; }

    public ConfigEntry<Color> PoorColor { get; set; }

    public ConfigEntry<Color> CommonColor { get; set; }

    public ConfigEntry<Color> UncommonColor { get; set; }

    public ConfigEntry<Color> RareColor { get; set; }

    public ConfigEntry<Color> EpicColor { get; set; }

    public ConfigEntry<Color> LegendaryColor { get; set; }
    #endregion

    #region Flea market
    public ConfigEntry<bool> AlwaysShowFleaPrice { get; set; }

    public ConfigEntry<decimal> FleaPriceMultiplicand { get; set; }

    public bool IncludeFleaTax
    {
        get
        {
            if (FleaTaxToggleMode.IsEnabled())
            {
                return _includeFleaTaxConfigEntry.IsEnabled()
                    && FleaTaxToggleKey.GetValue().IsPressed();
            }

            return _includeFleaTaxConfigEntry.IsEnabled();
        }
        set
        {
            _includeFleaTaxConfigEntry.Value = value;
        }
    }

    public bool ShowFleaTax
    {
        get
        {
            if (FleaTaxToggleMode.IsEnabled())
            {
                return _showFleaTaxConfigEntry.IsEnabled()
                    && FleaTaxToggleKey.GetValue().IsPressed();
            }

            return _showFleaTaxConfigEntry.IsEnabled();
        }
        set
        {
            _showFleaTaxConfigEntry.Value = value;
        }
    }

    public ConfigEntry<bool> FleaTaxToggleMode { get; set; }

    public ConfigEntry<KeyboardShortcut> FleaTaxToggleKey { get; set; }

    public ConfigEntry<int> UpdateInterval { get; set; }

    public ConfigEntry<bool> UpdateDuringRaid { get; set; }
    #endregion
}