using EFT.InventoryLogic;
using EFT.UI;
using SPT.Reflection.Patching;
using SwiftXP.SPT.ShowMeTheMoney.Client.Contexts;
using System.Reflection;
using UnityEngine;
using System;
using System.Text;
using HarmonyLib;
using SwiftXP.SPT.Common.Sessions;
using SwiftXP.SPT.ShowMeTheMoney.Client.Enums;
using TMPro;
using SwiftXP.SPT.Common.ConfigurationManager;
using System.Linq;
using SwiftXP.SPT.ShowMeTheMoney.Client.Services;
using System.Globalization;
using SwiftXP.SPT.ShowMeTheMoney.Client.Utilities;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;
using SwiftXP.SPT.ShowMeTheMoney.Client.Contexts.Holders;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class SimpleTooltipShowPatch : ModulePatch
{
    private static string? s_patchText;

    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(SimpleTooltip), x => x.Name == nameof(SimpleTooltip.Show) && x.GetParameters()[0].Name == "text");

    [PatchPrefix]
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public static void PatchPrefix(SimpleTooltip __instance, ref string text, Vector2? offset, ref float delay, float? maxWidth)
#pragma warning restore CA1707 // Identifiers should not contain underscores
    {
        try
        {
            if (!AreTooltipRequirementsMeet(text))
                return;

            Instance = __instance;

            SetToolTipDelay(ref delay);

            bool success = TryShowPriceInformation(out string? priceInformationText, out double? highestComparePrice);
            if (success)
            {
                if (PluginContextHolder.Current!.Configuration!.EnableColorCoding.IsEnabled()
                    && (PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.ItemName
                        || PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.Both))
                {
                    if (PluginContextHolder.Current.HoveredItem is not null)
                        SetColorCoding(ref text, PluginContextHolder.Current.HoveredItem.LocalizedName(), highestComparePrice);
                }

                s_patchText = priceInformationText;
                text += s_patchText;
                PatchIsActive = true;
            }
        }
        catch (Exception exception)
        {
            PluginContextHolder.Current.SptLogger?
                .LogException(exception);
        }
    }

    public static void Update()
    {
        try
        {
            if (Instance is not null && PatchIsActive)
            {
                string? instanceText = GetTextOfInstance();
                if (instanceText is not null)
                {
                    bool success = TryShowPriceInformation(out string? priceInformationText, out double? _);
                    if (success)
                    {
                        string newTooltipText = instanceText.Replace(s_patchText, priceInformationText);

                        s_patchText = priceInformationText;
                        Instance.SetText(newTooltipText);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            PluginContextHolder.Current.SptLogger?
                .LogException(exception);
        }
    }

    public static void OnClose()
    {
        Instance = null;
        s_patchText = null;
        PatchIsActive = false;
    }

    private static bool AreTooltipRequirementsMeet(in string tooltipText)
    {
        return PluginContextHolder.Current!.Configuration!.EnablePlugin.IsEnabled()
            && !PluginContextHolder.Current.DisableTemporary
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

    private static bool TryShowPriceInformation(out string? priceInformationText, out double? highestComparePrice)
    {
        highestComparePrice = null;

        StringBuilder textToAppendToTooltip = new();

        TooltipFontSize fontSize = PluginContextHolder.Current!.Configuration!.FontSize.GetValue();
        if (fontSize != TooltipFontSize.Normal)
            textToAppendToTooltip.Append($"<size={(int)fontSize}%>");

        if (PluginContextHolder.Current!.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("<i>");

        Item? item = PluginContextHolder.Current.HoveredItem;
        if (item is not null && ItemMeetsRequirements(item))
        {
            ShowTradeAndFleaPriceInformation(item, ref textToAppendToTooltip, out highestComparePrice);

            if (PluginContextHolder.Current!.Configuration!.ShowWeaponModsPrice.IsEnabled() && item is Weapon weapon)
                ShowWeaponModsPriceInformation(weapon, ref textToAppendToTooltip);

            if (PluginContextHolder.Current!.Configuration!.ShowArmorPlatesPrice.IsEnabled() && (item.GetItemComponent<ArmorHolderComponent>()?.MoveAbleArmorPlates?.Any() ?? false))
                ShowArmorPlatesPriceInformation(item, ref textToAppendToTooltip);
        }

        if (PluginContextHolder.Current!.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("</i>");

        if (fontSize != TooltipFontSize.Normal)
            textToAppendToTooltip.Append("</size>");

        priceInformationText = textToAppendToTooltip.ToString();

        return highestComparePrice is not null;
    }

    private static void ShowTradeAndFleaPriceInformation(Item item, ref StringBuilder textToAppendToTooltip, out double? highestComparePrice)
    {
        highestComparePrice = null;

        TradeItem tradeItem = new(item);
        bool hasTraderPrice = false;
        bool hasFleaPrice = false;
        bool isCombinedPrice = false;

        if (PluginContextHolder.Current!.Configuration!.EnableTraderPrices.IsEnabled())
            hasTraderPrice = TraderPriceService.Instance.GetBestTraderPrice(tradeItem);

        if (PluginContextHolder.Current!.Configuration!.EnableFleaPrices.IsEnabled()
            && (SptSession.Session.RagFair.Available || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled())
            && (!RagFairClass.Settings.isOnlyFoundInRaidAllowed
                || (RagFairClass.Settings.isOnlyFoundInRaidAllowed && tradeItem.Item.MarkedAsSpawnedInSession)
                || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled()))
        {
            hasFleaPrice = FleaPriceUtility.GetFleaPrice(tradeItem, PluginContextHolder.Current!.Configuration!.IncludeFleaTax, out isCombinedPrice);
        }

        if (hasTraderPrice)
        {
            ShowPrice(tradeItem, tradeItem.TraderPrice!, tradeItem.FleaPrice, textToAppendToTooltip);
            highestComparePrice = tradeItem.TraderPrice!.GetComparePriceInRouble();
        }

        if (hasFleaPrice)
        {
            ShowPrice(tradeItem, tradeItem.FleaPrice!, tradeItem.TraderPrice, textToAppendToTooltip, isCombinedPrice);

            if (highestComparePrice is null || tradeItem.FleaPrice!.GetComparePriceInRouble() > tradeItem.TraderPrice!.GetComparePriceInRouble())
                highestComparePrice = tradeItem.FleaPrice!.GetComparePriceInRouble();
        }
    }

    private static void ShowWeaponModsPriceInformation(Weapon weapon, ref StringBuilder textToAppendToTooltip)
    {
        double modsPrice = 0d;

        foreach (Mod mod in weapon.Mods)
        {
            Mod clonedMod = mod.CloneItem();
            clonedMod.Slots = [];

            TradeItem modTradeItem = new(clonedMod);

            bool modHasTraderPrice = false;
            bool modHasFleaPrice = false;

            if (PluginContextHolder.Current!.Configuration!.EnableTraderPrices.IsEnabled())
                modHasTraderPrice = TraderPriceService.Instance.GetBestTraderPrice(modTradeItem);

            if (PluginContextHolder.Current!.Configuration!.EnableFleaPrices.IsEnabled()
                && (SptSession.Session.RagFair.Available || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled())
                && (!RagFairClass.Settings.isOnlyFoundInRaidAllowed
                    || (RagFairClass.Settings.isOnlyFoundInRaidAllowed && mod.MarkedAsSpawnedInSession)
                    || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled()))
            {
                modHasFleaPrice = FleaPriceUtility.GetFleaPrice(modTradeItem, PluginContextHolder.Current!.Configuration!.IncludeFleaTax);
            }

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
            textToAppendToTooltip.Append($"<br>{"APCTab/Mods".Localized(null)}: {FormatPrice(modsPrice)}");
    }

    private static void ShowArmorPlatesPriceInformation(Item armorItem, ref StringBuilder textToAppendToTooltip)
    {
        double platesPrice = 0d;

        ArmorHolderComponent armorHolderComponent = armorItem.GetItemComponent<ArmorHolderComponent>();
        foreach (ArmorPlateItemClass armorPlateItemClass in armorHolderComponent.MoveAbleArmorPlates)
        {
            ArmorPlateItemClass clonedPlate = armorPlateItemClass.CloneItem();
            TradeItem plateTradeItem = new(clonedPlate);

            bool modHasTraderPrice = false;
            bool modHasFleaPrice = false;

            if (PluginContextHolder.Current!.Configuration!.EnableTraderPrices.IsEnabled())
                modHasTraderPrice = TraderPriceService.Instance.GetBestTraderPrice(plateTradeItem);

            if (PluginContextHolder.Current!.Configuration!.EnableFleaPrices.IsEnabled()
                && (SptSession.Session.RagFair.Available || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled())
                && (!RagFairClass.Settings.isOnlyFoundInRaidAllowed
                    || (RagFairClass.Settings.isOnlyFoundInRaidAllowed && armorPlateItemClass.MarkedAsSpawnedInSession)
                    || PluginContextHolder.Current!.Configuration!.AlwaysShowFleaPrice.IsEnabled()))
            {
                modHasFleaPrice = FleaPriceUtility.GetFleaPrice(plateTradeItem, PluginContextHolder.Current!.Configuration!.IncludeFleaTax);
            }

            if (modHasTraderPrice && modHasFleaPrice)
            {
                if (plateTradeItem.TraderPrice!.GetComparePriceInRouble() > plateTradeItem.FleaPrice!.GetComparePriceInRouble())
                    platesPrice += plateTradeItem.TraderPrice!.GetTotalPriceInRouble();
                else
                    platesPrice += plateTradeItem.FleaPrice!.GetTotalPriceInRouble();
            }
            else if (modHasFleaPrice)
            {
                platesPrice += plateTradeItem.FleaPrice!.GetTotalPriceInRouble();
            }
            else
            {
                platesPrice += plateTradeItem.TraderPrice!.GetTotalPriceInRouble();
            }
        }

        if (platesPrice > 0)
            textToAppendToTooltip.Append($"<br>{"67600929bd0a0549d70993f6 ShortName".Localized(null)}: {FormatPrice(platesPrice)}");
    }

    private static void SetColorCoding(ref string text, string textToReplace, double? highestComparePrice)
    {
        if (highestComparePrice is not null)
        {
            Item item = PluginContextHolder.Current.HoveredItem!;
            if (PluginContextHolder.Current!.Configuration!.UseCaliberPenetrationPower.IsEnabled()
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
                case var _ when penetrationPower < (double)PluginContextHolder.Current!.Configuration!.PoorPenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.PoorColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)PluginContextHolder.Current!.Configuration!.CommonPenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.CommonColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)PluginContextHolder.Current!.Configuration!.UncommonPenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.UncommonColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)PluginContextHolder.Current!.Configuration!.RarePenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.RareColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower < (double)PluginContextHolder.Current!.Configuration!.EpicPenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.EpicColor.GetRGBHexCode();
                    break;

                case var _ when penetrationPower >= (double)PluginContextHolder.Current!.Configuration!.EpicPenetrationValue.GetValue():
                    colorCoding = PluginContextHolder.Current!.Configuration!.LegendaryColor.GetRGBHexCode();
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
            case var _ when highestComparePrice < (double)PluginContextHolder.Current!.Configuration!.PoorValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.PoorColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)PluginContextHolder.Current!.Configuration!.CommonValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.CommonColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)PluginContextHolder.Current!.Configuration!.UncommonValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.UncommonColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)PluginContextHolder.Current!.Configuration!.RareValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.RareColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice < (double)PluginContextHolder.Current!.Configuration!.EpicValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.EpicColor.GetRGBHexCode();
                break;

            case var _ when highestComparePrice >= (double)PluginContextHolder.Current!.Configuration!.EpicValue.GetValue():
                colorCoding = PluginContextHolder.Current!.Configuration!.LegendaryColor.GetRGBHexCode();
                break;
        }

        if (colorCoding is not null)
            text = text.Replace(textToReplace, $"<color=#{colorCoding}>{textToReplace}</color>");
    }

    private static void SetToolTipDelay(ref float delay)
    {
        delay = (float)PluginContextHolder.Current!.Configuration!.ToolTipDelay.GetValue();
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

    private static void ShowPrice(TradeItem tradeItem, TradePrice tradePriceA, TradePrice? tradePriceB, StringBuilder text, bool isCombinedPrice = false)
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

        if ((tradeItem.ItemSlotCount > 1 || tradeItem.Item.StackObjectsCount > 1) && PluginContextHolder.Current!.Configuration!.ShowPricePerSlot.IsEnabled())
        {
            string slotPrice = FormatPrice(tradePriceA.GetComparePrice(), tradePriceA.CurrencySymbol);

            if (PluginContextHolder.Current!.Configuration!.EnableColorCoding.IsEnabled()
                && (PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.Price || PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.Both))
            {
                SetColorCoding(ref slotPrice, slotPrice, tradePriceA.GetComparePriceInRouble());
            }

            text.Append($"{slotPrice} {"Total".Localized(null)}: ");
        }

        string totalPrice = FormatPrice(tradePriceA.GetTotalPrice(), tradePriceA.CurrencySymbol);
        if (isBestPrice)
            totalPrice = $"<b>{totalPrice}</b>";

        if (PluginContextHolder.Current!.Configuration!.EnableColorCoding.IsEnabled()
            && (PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.Price || PluginContextHolder.Current!.Configuration!.ColorCodingMode.GetValue() == ColorCodingMode.Both)
            && ((tradeItem.ItemSlotCount == 1 && tradeItem.Item.StackObjectsCount == 1) || !PluginContextHolder.Current!.Configuration!.ShowPricePerSlot.IsEnabled()))
        {
            SetColorCoding(ref totalPrice, totalPrice, tradePriceA.GetComparePriceInRouble());
        }

        text.Append(totalPrice);

        if (isCombinedPrice)
            text.Append(" (+)");

        if (PluginContextHolder.Current!.Configuration!.ShowFleaTax && tradePriceA.HasTax())
            text.Append($" {"ragfair/Fee".Localized(null)}: {FormatPrice(tradePriceA.GetTotalTax())}");
    }

    private static string GetBestTradeColor()
    {
        return ColorUtility.ToHtmlStringRGB(PluginContextHolder.Current!.Configuration!.BestTradeColor.GetValue());
    }

    private static string FormatPrice(double val, string currency = "₽")
    {
        val = Math.Round(val);

        if (val >= 100000000)
            return $"{currency}{(val / 1000000).ToString("#,0M", CultureInfo.InvariantCulture)}";

        if (val >= 10000000)
            return $"{currency}{(val / 1000000).ToString("0.#", CultureInfo.InvariantCulture)}M";

        if (val >= 100000)
            return $"{currency}{(val / 1000).ToString("#,0k", CultureInfo.InvariantCulture)}";

        if (val >= 10000)
            return $"{currency}{(val / 1000).ToString("0.#", CultureInfo.InvariantCulture)}k";

        return $"{currency}{val:#,0}";
    }

    public static SimpleTooltip? Instance { get; private set; }

    public static bool PatchIsActive { get; private set; }
}