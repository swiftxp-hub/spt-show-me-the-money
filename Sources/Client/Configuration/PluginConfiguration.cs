using BepInEx.Configuration;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.Notifications;
using UnityEngine;
using System;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Client.Enums;
using SwiftXP.SPT.ShowMeTheMoney.Client.Services;
using System.Threading.Tasks;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;

public class PluginConfiguration
{
    private ConfigEntry<bool> includeFleaTaxConfigEntry;

    private ConfigEntry<bool> showFleaTaxConfigEntry;

    public PluginConfiguration(ConfigFile configFile)
    {
        // --- 1. Main settings
        this.EnablePlugin = configFile.BindConfiguration("1. Main settings", "Enable plug-in", true, $"Enable or disable the plug-in.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 5);
        this.EnableTraderPrices = configFile.BindConfiguration("1. Main settings", "Enable trader price(s)", true, $"Enable the trader price(s) in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 4);
        this.EnableFleaPrices = configFile.BindConfiguration("1. Main settings", "Enable flea price(s)", true, $"Enable the flea price(s) in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 3);
        this.ShowPricePerSlot = configFile.BindConfiguration("1. Main settings", "Show price-per-slot", true, $"Show the price-per-slot in the tool-tip. The mod continues to calculate in the background using price-per-slot, even if the display is deactivated.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 2);
        this.ShowWeaponModsPrice = configFile.BindConfiguration("1. Main settings", "Show weapon-mods price", true, $"Show the total price of all modifications installed in a weapon.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 1);
        this.ToolTipDelay = configFile.BindConfiguration("1. Main settings", "Tool-Tip delay", 0.0m, $"Delays the tool-tip for x seconds.{Environment.NewLine}{Environment.NewLine}(Plug-In Default: 0, EFT Default: 0.6)", 0);

        // --- 2. Currency conversion
        this.RoublesOnly = configFile.BindConfiguration("2. Currency conversion", "Roubles only", false, $"Only sales prices in roubles will be considered. Basically no longer displays trades from traders who do not buy in rubles.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 0);

        // --- 3. Appearance
        this.BestTradeColor = configFile.BindConfiguration("3. Appearance", "Best trade color", new Color(1.000f, 1.000f, 1.000f), $"Defines the color used to highlight the best trade, trader or flea.{Environment.NewLine}{Environment.NewLine}(Default: R 255, G 255, B 255)", 2);
        this.FontSize = configFile.BindConfiguration("3. Appearance", "Font size", TooltipFontSizeEnum.Smaller, $"Changes the font size of the price(s).{Environment.NewLine}{Environment.NewLine}(Default: Smaller)", 1);
        this.RenderInItalics = configFile.BindConfiguration("3. Appearance", "Italics", false, $"Renders the price(s) in italics.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 0);

        // --- 4. Color coding
        this.EnableColorCoding = configFile.BindConfiguration("4. Color coding", "Enable color coding (based on price-per-slot)", true, $"Uses color coding to give an quick and easy indicator how valueable an item is. Always based on price-per-slot, except for ammunition, if you activate the respective feature. Default colors are from WoW.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 18);
        this.ColorCodingMode = configFile.BindConfiguration("4. Color coding", "Color coding mode", ColorCodingModeEnum.Both, $"Defines the color coding mode.{Environment.NewLine}{Environment.NewLine}(Default: Both)", 17);

        this.PoorValue = configFile.BindConfiguration("4. Color coding", "Poor value (smaller than)", 800m, "(Default: 800)", 16);
        this.CommonValue = configFile.BindConfiguration("4. Color coding", "Common value (smaller than)", 8000m, "(Default: 8000)", 15);
        this.UncommonValue = configFile.BindConfiguration("4. Color coding", "Uncommon value (smaller than)", 15000m, "(Default: 15000)", 14);
        this.RareValue = configFile.BindConfiguration("4. Color coding", "Rare value (smaller than)", 24000m, "(Default: 24000)", 13);
        this.EpicValue = configFile.BindConfiguration("4. Color coding", "Epic value (smaller than) - everything above that is considered legendary", 54000m, "(Default: 54000)", 12);

        this.UseCaliberPenetrationPower = configFile.BindConfiguration("4. Color coding", "Use penetration power instead of price value for color coding of ammunition", true, $"Uses the caliber penetration power value instead of the price value for color coding of ammunition.{Environment.NewLine}{Environment.NewLine}(Default: Enabled)", 11);

        this.PoorPenetrationValue = configFile.BindConfiguration("4. Color coding", "Poor penetration value (smaller than)", 15m, "(Default: 15)", 10);
        this.CommonPenetrationValue = configFile.BindConfiguration("4. Color coding", "Common penetration value (smaller than)", 25m, "(Default: 25)", 9);
        this.UncommonPenetrationValue = configFile.BindConfiguration("4. Color coding", "Uncommon penetration value (smaller than)", 34m, "(Default: 34)", 8);
        this.RarePenetrationValue = configFile.BindConfiguration("4. Color coding", "Rare penetration value (smaller than)", 43m, "(Default: 43)", 7);
        this.EpicPenetrationValue = configFile.BindConfiguration("4. Color coding", "Epic penetration value (smaller than) - everything above that is considered legendary", 55m, "(Default: 55)", 6);

        this.PoorColor = configFile.BindConfiguration("4. Color coding", "Poor color", new Color(0.62f, 0.62f, 0.62f), "(Default: R 157, G 157, B 157)", 5);
        this.CommonColor = configFile.BindConfiguration("4. Color coding", "Common color", new Color(1f, 1f, 1f), "(Default: R 255, G 255, B 255)", 4);
        this.UncommonColor = configFile.BindConfiguration("4. Color coding", "Uncommon color", new Color(0.12f, 1f, 0f), "(Default: R 30, G 255, B 0)", 3);
        this.RareColor = configFile.BindConfiguration("4. Color coding", "Rare color", new Color(0f, 0.44f, 0.87f), "(Default: R 0, G 112, B 221)", 2);
        this.EpicColor = configFile.BindConfiguration("4. Color coding", "Epic color", new Color(0.64f, 0.21f, 0.93f), "(Default: R 163, G 53, B 238)", 1);
        this.LegendaryColor = configFile.BindConfiguration("4. Color coding", "Legendary color", new Color(1f, 0.5f, 0f), "(Default: R 255, G 128, B 0)", 0);

        // --- 5. Flea market
        this.ShowFleaPriceDespiteNotUnlocked = configFile.BindConfiguration("5. Flea market", "Show flea price despite not unlocked", false, $"Show the flea price of an item even if the flea market is not yet unlocked.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 6);
        this.FleaPriceMultiplicand = configFile.BindConfiguration("5. Flea market", "Flea price multiplicand", 1.0m, $"Sets the multiplicand by which the average flea market price is multiplied and then displayed in the tooltip. The following calculation is performed:{Environment.NewLine}{Environment.NewLine}Average flea market price of the item * Flea price multiplicand{Environment.NewLine}{Environment.NewLine}(Default: 1.0)", 5);
        this.includeFleaTaxConfigEntry = configFile.BindConfiguration("5. Flea market", "Include flea tax", false, $"Determines whether taxes for the flea market are included in the flea price.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 4);
        this.showFleaTaxConfigEntry = configFile.BindConfiguration("5. Flea market", "Show flea tax", false, $"Show the flea tax in the tool-tip.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 3);
        this.FleaTaxToggleMode = configFile.BindConfiguration("5. Flea market", "Toggle-mode for flea tax", false, $"When toggle mode is activated, the flea tax is only displayed when the specified key or key combination is pressed.{Environment.NewLine}{Environment.NewLine}(Default: Disabled)", 2);
        this.FleaTaxToggleKey = configFile.BindConfiguration("5. Flea market", "Toggle-mode key", new KeyboardShortcut(KeyCode.LeftAlt), $"Defines which key or key combination needs to be pressed to display the flea tax.{Environment.NewLine}{Environment.NewLine}(Default: LeftAlt)", 1);

        configFile.CreateButton(
            "5. Flea market",
            "Update flea prices",
            "Update now",
            "Pulls the current flea prices from your SPT server instance.",
            () =>
            {
                FleaPricesService.Instance.ForceUpdatePrices();
                NotificationsService.Instance.SendLongNotice("Flea prices will update as soon as possible...");
            },
            0
        );

        this.FleaTaxToggleMode.SettingChanged += (_, _) =>
        {
            if (this.FleaTaxToggleMode.IsEnabled())
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

    public ConfigEntry<decimal> ToolTipDelay { get; set; }
    #endregion

    #region Currency conversion
    public ConfigEntry<bool> RoublesOnly { get; set; }
    #endregion

    #region Appearance
    public ConfigEntry<Color> BestTradeColor { get; set; }

    public ConfigEntry<TooltipFontSizeEnum> FontSize { get; set; }

    public ConfigEntry<bool> RenderInItalics { get; set; }
    #endregion

    #region Color coding
    public ConfigEntry<bool> EnableColorCoding { get; set; }

    public ConfigEntry<ColorCodingModeEnum> ColorCodingMode { get; set; }

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
    public ConfigEntry<bool> ShowFleaPriceDespiteNotUnlocked { get; set; }

    public ConfigEntry<decimal> FleaPriceMultiplicand { get; set; }

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
        set
        {
            this.includeFleaTaxConfigEntry.Value = value;
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
        set
        {
            this.showFleaTaxConfigEntry.Value = value;
        }
    }

    public ConfigEntry<bool> FleaTaxToggleMode { get; set; }

    public ConfigEntry<KeyboardShortcut> FleaTaxToggleKey { get; set; }
    #endregion
}