using EFT.InventoryLogic;
using EFT.UI;
using SPT.Reflection.Patching;
using SwiftXP.SPT.ShowMeTheMoney.Models;
using System.Reflection;
using UnityEngine;
using System;
using System.Text;
using HarmonyLib;
using SwiftXP.SPT.Common.Sessions;
using SwiftXP.SPT.ShowMeTheMoney.Enums;
using TMPro;
using SwiftXP.SPT.Common.ConfigurationManager;
using System.Linq;
using SwiftXP.SPT.Common.Loggers;
using SwiftXP.SPT.ShowMeTheMoney.Services;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class SimpleTooltipShowPatch : ModulePatch
{
    private static string? patchText;

    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(SimpleTooltip), x => x.Name == nameof(SimpleTooltip.Show) && x.GetParameters()[0].Name == "text");

    [PatchPrefix]
    public static void PatchPrefix(SimpleTooltip __instance, ref string text, Vector2? offset, ref float delay, float? maxWidth)
    {
        try
        {
            if (!AreTooltipRequirementsMeet(text))
                return;

            Instance = __instance;

            SetToolTipDelay(ref delay);

            bool success = TryShowPriceInformations(out string? priceInformationText, out double? highestComparePrice);
            if (success)
            {
                if (Plugin.Configuration!.EnableColorCoding.IsEnabled()
                    && (Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.ItemName || Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.Both))
                {
                    SetColorCoding(ref text, Plugin.HoveredItem.LocalizedName(), highestComparePrice);
                }

                patchText = priceInformationText;
                text += patchText;
                IsActive = true;
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }
    }

    public static void Update()
    {
        try
        {
            if (Instance is not null && IsActive)
            {
                string? instanceText = GetTextOfInstance();
                if (instanceText is not null)
                {
                    bool success = TryShowPriceInformations(out string? priceInformationText, out double? _);
                    if (success)
                    {
                        string newTooltipText = instanceText.Replace(patchText, priceInformationText);

                        patchText = priceInformationText;
                        Instance.SetText(newTooltipText);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            SimpleSptLogger.Instance.LogException(exception);
        }
    }

    public static void OnClose()
    {
        Instance = null;
        patchText = null;
        IsActive = false;
    }

    private static bool AreTooltipRequirementsMeet(in string tooltipText)
    {
        return Plugin.Configuration!.EnablePlugin.IsEnabled()
            && !Plugin.DisableTemporary
            && !IsInsuredByTooltip(tooltipText)
            && !IsCheckmarkTooltip(tooltipText);
    }

    private static bool IsInsuredByTooltip(in string text)
    {
        if (text.Contains("Insured by".Localized(null), StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    private static bool IsCheckmarkTooltip(in string text)
    {
        if (text.Contains("STASH".Localized(null), StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (text.Contains("FoundInRaid".Localized(null), StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    private static bool TryShowPriceInformations(out string? priceInformationText, out double? highestComparePrice)
    {
        highestComparePrice = null;

        StringBuilder textToAppendToTooltip = new();

        TooltipFontSizeEnum fontSize = Plugin.Configuration!.FontSize.GetValue();
        if (fontSize != TooltipFontSizeEnum.Normal)
            textToAppendToTooltip.Append($"<size={(int)fontSize}%>");

        if (Plugin.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("<i>");

        Item? item = Plugin.HoveredItem;
        if (item is not null && ItemMeetsRequirements(item))
        {
            TradeItem tradeItem = new(item);

            bool hasTraderPrice = false;
            bool hasFleaPrice = false;

            if (Plugin.Configuration!.EnableTraderPrices.IsEnabled())
                hasTraderPrice = TraderPriceService.Instance.GetBestTraderPrice(tradeItem);

            if (Plugin.Configuration!.EnableFleaPrices.IsEnabled())
                hasFleaPrice = FleaPriceService.Instance.GetFleaPrice(tradeItem, Plugin.Configuration!.IncludeFleaTax);

            if (hasTraderPrice)
            {
                ShowPrice(tradeItem, tradeItem.TraderPrice!, tradeItem.FleaPrice, textToAppendToTooltip);
                highestComparePrice = tradeItem.TraderPrice!.GetComparePriceInRouble();
            }

            if (hasFleaPrice)
            {
                ShowPrice(tradeItem, tradeItem.FleaPrice!, tradeItem.TraderPrice, textToAppendToTooltip);

                if (highestComparePrice is null || tradeItem.FleaPrice!.GetComparePriceInRouble() > tradeItem.TraderPrice!.GetComparePriceInRouble())
                    highestComparePrice = tradeItem.FleaPrice!.GetComparePriceInRouble();
            }

            if (item is Weapon weapon)
            {
                double modsPrice = 0;
                foreach (Mod mod in weapon.Mods)
                {
                    SimpleSptLogger.Instance.LogInfo(mod.LocalizedName());

                    TradeItem modTradeItem = new TradeItem(mod);

                    bool modHasTraderPrice = false;
                    bool modHasFleaPrice = false;

                    if (Plugin.Configuration!.EnableTraderPrices.IsEnabled())
                        modHasTraderPrice = TraderPriceService.Instance.GetBestTraderPrice(modTradeItem);

                    if (Plugin.Configuration!.EnableFleaPrices.IsEnabled())
                        modHasFleaPrice = FleaPriceService.Instance.GetFleaPrice(modTradeItem, Plugin.Configuration!.IncludeFleaTax);

                    if (modHasTraderPrice && modHasFleaPrice)
                    {
                        if (modTradeItem.TraderPrice!.GetComparePriceInRouble() > modTradeItem.FleaPrice!.GetComparePriceInRouble())
                            modsPrice += modTradeItem.TraderPrice!.GetTotalPriceInRouble();
                        else
                            modsPrice += modTradeItem.FleaPrice!.GetTotalPriceInRouble();
                    }
                    else if (modHasFleaPrice)
                    {
                        modsPrice += modTradeItem.FleaPrice!.GetTotalPriceInRouble();
                    }
                    else
                    {
                        modsPrice += modTradeItem.TraderPrice!.GetTotalPriceInRouble();
                    }
                }

                if (modsPrice > 0)
                {
                    SimpleSptLogger.Instance.LogInfo(modsPrice);
                    textToAppendToTooltip.Append($"<br>Weapon-Mods: {FormatPrice(modsPrice)}");
                }
            }
        }

        if (Plugin.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("</i>");

        if (fontSize != TooltipFontSizeEnum.Normal)
            textToAppendToTooltip.Append("</size>");

        priceInformationText = textToAppendToTooltip.ToString();

        return highestComparePrice is not null;
    }

    private static void SetColorCoding(ref string text, string textToReplace, double? highestComparePrice)
    {
        if (highestComparePrice is not null)
        {
            Item item = Plugin.HoveredItem!;
            if (Plugin.Configuration!.UseCaliberPenetrationPower.IsEnabled()
                && (item is AmmoBox || item is AmmoItemClass))
            {
                SetColorCodingForAmmunition(item, ref text, textToReplace);
            }
            else
            {
                SetColorCodingForItem(ref text, textToReplace, highestComparePrice);
            }
        }
    }

    private static void SetColorCodingForAmmunition(Item item, ref string text, string textToReplace)
    {
        string? colorCoding = null;

        int? penetrationPower = null;
        if (item is AmmoBox ammoBox)
        {
            AmmoItemClass? ammoItemClass = ammoBox.Cartridges.Items.First() as AmmoItemClass;
            penetrationPower = ammoItemClass?.PenetrationPower;
        }
        else if (item is AmmoItemClass ammoItemClass)
        {
            penetrationPower = ammoItemClass?.PenetrationPower;
        }

        if (penetrationPower is not null)
        {
            switch (penetrationPower)
            {
                case var _ when penetrationPower < (double)Plugin.Configuration!.PoorPenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.PoorColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)Plugin.Configuration!.CommonPenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.CommonColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)Plugin.Configuration!.UncommonPenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.UncommonColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)Plugin.Configuration!.RarePenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.RareColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)Plugin.Configuration!.EpicPenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.EpicColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower >= (double)Plugin.Configuration!.EpicPenetrationValue.GetValue():
                    colorCoding = Plugin.Configuration!.LegendaryColor.GetRGBHexCode();
                    break;
            }
        }

        if (colorCoding is not null)
            text = text.Replace(textToReplace, $"<color=#{colorCoding}>{textToReplace}</color>");
    }

    private static void SetColorCodingForItem(ref string text, string textToReplace, double? highestComparePrice)
    {
        string? colorCoding = null;

        switch (highestComparePrice)
        {
            case var _ when highestComparePrice < (double)Plugin.Configuration!.PoorValue.GetValue():
                colorCoding = Plugin.Configuration!.PoorColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)Plugin.Configuration!.CommonValue.GetValue():
                colorCoding = Plugin.Configuration!.CommonColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)Plugin.Configuration!.UncommonValue.GetValue():
                colorCoding = Plugin.Configuration!.UncommonColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)Plugin.Configuration!.RareValue.GetValue():
                colorCoding = Plugin.Configuration!.RareColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)Plugin.Configuration!.EpicValue.GetValue():
                colorCoding = Plugin.Configuration!.EpicColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice >= (double)Plugin.Configuration!.EpicValue.GetValue():
                colorCoding = Plugin.Configuration!.LegendaryColor.GetRGBHexCode();
                break;
        }

        if (colorCoding is not null)
            text = text.Replace(textToReplace, $"<color=#{colorCoding}>{textToReplace}</color>");
    }

    private static void SetToolTipDelay(ref float delay)
    {
        delay = (float)Plugin.Configuration!.ToolTipDelay.GetValue();
    }

    private static bool ItemMeetsRequirements(Item item)
    {
        return SptSession.Session.Profile.Examined(item)
            && (!item.IsContainer || (item.IsContainer && item.IsEmpty()))
            && !(item.Owner.OwnerType != EOwnerType.Profile && item.Owner.GetType() == typeof(TraderControllerClass));
    }

    private static string? GetTextOfInstance()
    {
        FieldInfo? fieldInfo = typeof(SimpleTooltip)
            .GetField("_label", BindingFlags.NonPublic | BindingFlags.Instance);

        TextMeshProUGUI? textMesh = fieldInfo?.GetValue(Instance) as TextMeshProUGUI;

        return textMesh?.text ?? null;
    }

    private static void ShowPrice(TradeItem tradeItem, TradePrice tradePriceA, TradePrice? tradePriceB, StringBuilder text)
    {
        text.Append("<br>");

        bool isBestPrice = false;
        if (tradePriceB is not null)
        {
            if (tradePriceA.GetComparePriceInRouble() > tradePriceB.GetComparePriceInRouble())
                isBestPrice = true;
        }
        else
        {
            isBestPrice = true;
        }

        if (isBestPrice)
            text.Append($"<color=#{GetBestTradeColor()}>{tradePriceA.TraderName}</color>: ");
        else
            text.Append($"{tradePriceA.TraderName}: ");

        if ((tradeItem.ItemSlotCount > 1 || tradeItem.Item.StackObjectsCount > 1) && Plugin.Configuration!.ShowPricePerSlot.IsEnabled())
        {
            string slotPrice = FormatPrice(tradePriceA.GetComparePrice(), tradePriceA.CurrencySymbol);

            if (Plugin.Configuration!.EnableColorCoding.IsEnabled()
                && (Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.Price || Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.Both))
            {
                SetColorCoding(ref slotPrice, slotPrice, tradePriceA.GetComparePriceInRouble());
            }

            text.Append($"{slotPrice} {"Total".Localized(null)}: ");
        }

        string totalPrice = FormatPrice(tradePriceA.GetTotalPrice(), tradePriceA.CurrencySymbol);
        if (isBestPrice)
            totalPrice = $"<b>{totalPrice}</b>";

        if (Plugin.Configuration!.EnableColorCoding.IsEnabled()
            && (Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.Price || Plugin.Configuration!.ColorCodingMode.GetValue() == ColorCodingModeEnum.Both)
            && ((tradeItem.ItemSlotCount == 1 && tradeItem.Item.StackObjectsCount == 1) || !Plugin.Configuration!.ShowPricePerSlot.IsEnabled()))
        {
            SetColorCoding(ref totalPrice, totalPrice, tradePriceA.GetComparePriceInRouble());
        }

        text.Append(totalPrice);

        if (Plugin.Configuration!.ShowFleaTax && tradePriceA.HasTax())
            text.Append($" {"ragfair/Fee".Localized(null)}: {FormatPrice(tradePriceA.GetTotalTax())}");
    }

    private static string GetBestTradeColor()
    {
        return ColorUtility.ToHtmlStringRGB(Plugin.Configuration!.BestTradeColor.GetValue());
    }

    private static string FormatPrice(double val, string currency = "₽")
    {
        val = Math.Round(val);

        if (val >= 100000000)
            return $"{currency}{(val / 1000000).ToString("#,0M")}";

        if (val >= 10000000)
            return $"{currency}{(val / 1000000).ToString("0.#")}M";

        if (val >= 100000)
            return $"{currency}{(val / 1000).ToString("#,0k")}";

        if (val >= 10000)
            return $"{currency}{(val / 1000).ToString("0.#")}k";

        return $"{currency}{val:#,0}";
    }

    public static SimpleTooltip? Instance { get; private set; }

    public static bool IsActive { get; private set; } = false;
}