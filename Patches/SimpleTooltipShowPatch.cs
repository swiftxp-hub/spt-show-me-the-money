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
using EFT;
using SwiftXP.SPT.ShowMeTheMoney.Enums;
using TMPro;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.Common.Constants;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class SimpleTooltipShowPatch : ModulePatch
{
    private static SimpleTooltip? instance;

    private static string? patchText;

    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(SimpleTooltip), x => x.Name == "Show" && x.GetParameters()[0].Name == "text");

    [PatchPrefix]
    public static void PatchPrefix(SimpleTooltip __instance, ref string text, Vector2? offset, ref float delay, float? maxWidth)
    {
        if (!AreTooltipRequirementsMeet(text))
            return;

        instance = __instance;

        try
        {
            SetToolTipDelay(ref delay);

            bool success = TryShowPriceInformations(out string? priceInformationText, out double? highestPrice);
            if (success)
            {
                patchText = priceInformationText;
                text += patchText;
                IsActive = true;
            }
        }
        catch (Exception exception)
        {
            Plugin.SimpleSptLogger.LogError($"An unexpected error occured. Message: {exception.Message}");
        }
    }

    public static void Update()
    {
        if (instance is not null && IsActive)
        {
            string? instanceText = GetTextOfInstance();
            if (instanceText is not null)
            {
                bool success = TryShowPriceInformations(out string? priceInformationText, out double? highestPrice);
                if (success)
                {
                    string newTooltipText = instanceText.Replace(patchText, priceInformationText);

                    patchText = priceInformationText;
                    instance.SetText(newTooltipText);
                }
            }
        }
    }

    public static void OnClose()
    {
        instance = null;
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
        if (text.Contains("Stash".Localized(null), StringComparison.InvariantCultureIgnoreCase))
            return true;

        if (text.Contains("Found in raid".Localized(null), StringComparison.InvariantCultureIgnoreCase))
            return true;

        return false;
    }

    private static bool TryShowPriceInformations(out string? priceInformationText, out double? highestPrice)
    {
        highestPrice = null;

        StringBuilder textToAppendToTooltip = new();

        if (Plugin.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("<i>");

        Item? item = Plugin.HoveredItem;
        if (item is not null && ItemMeetsRequirements(item))
        {
            TradeItem tradeItem = CreateTradeItem(item);

            bool hasTraderPrice = false;
            bool hasFleaPrice = false;

            if (Plugin.Configuration!.ShowTraderPrices.IsEnabled())
                hasTraderPrice = GetBestTraderPrice(tradeItem);

            if (Plugin.Configuration!.ShowFleaPrices.IsEnabled())
                hasFleaPrice = GetFleaPrice(tradeItem);

            if (hasTraderPrice)
            {
                ShowPrice(tradeItem, tradeItem.TraderPrice!, tradeItem.FleaPrice, textToAppendToTooltip);
                highestPrice = tradeItem.TraderPrice!.GetTotalPriceInRouble();
            }

            if (hasFleaPrice)
            {
                ShowPrice(tradeItem, tradeItem.FleaPrice!, tradeItem.TraderPrice, textToAppendToTooltip);

                if (highestPrice is null || tradeItem.FleaPrice!.GetComparePriceInRouble() > tradeItem.TraderPrice!.GetComparePriceInRouble())
                    highestPrice = tradeItem.FleaPrice!.GetTotalPriceInRouble();
            }
        }

        if (Plugin.Configuration!.RenderInItalics.IsEnabled())
            textToAppendToTooltip.Append("</i>");

        priceInformationText = textToAppendToTooltip.ToString();

        return highestPrice is not null;
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

        TextMeshProUGUI? textMesh = fieldInfo?.GetValue(instance) as TextMeshProUGUI;

        return textMesh?.text ?? null;
    }

    private static TradeItem CreateTradeItem(Item item)
    {
        XYCellSizeStruct itemSize = item.CalculateCellSize();
        int itemSlotCount = itemSize.X * itemSize.Y;

        TradeItem tradeItem = new TradeItem(
            item,
            itemSlotCount
        );

        return tradeItem;
    }

    private static bool GetBestTraderPrice(TradeItem tradeItem)
    {
        TradePrice? highestTraderPrice = null;
        foreach (TraderClass trader in SptSession.Session.Traders)
        {
            if (IsTraderAvailable(trader))
            {
                TraderClass.GStruct264? singleObjectPrice = null;
                TraderClass.GStruct264? totalPrice = null;

                bool hasPrice = GetTraderUserItemPrice(trader, tradeItem, out singleObjectPrice, out totalPrice);
                if (hasPrice && (!Plugin.Configuration!.RoublesOnly.IsEnabled() || singleObjectPrice!.Value.CurrencyId.ToString() == SptConstants.CurrencyIds.Roubles))
                {
                    MongoID? currencyId = singleObjectPrice!.Value.CurrencyId;
                    double? currencyCourse = GetCurrencyCourse(trader, currencyId);
                    double itemPrice = singleObjectPrice.Value.Amount;

                    double? totalItemPrice = totalPrice != null ? totalPrice.Value.Amount : null;

                    TradePrice traderPrice =
                        CreateTradePrice(
                            tradeItem,
                            trader.LocalizedName,
                            singleObjectPrice.Value.Amount,
                            totalItemPrice,
                            currencyCourse,
                            currencyId
                        );

                    if (highestTraderPrice == null || traderPrice.GetComparePriceInRouble() > highestTraderPrice.GetComparePriceInRouble())
                        highestTraderPrice = traderPrice;
                }
            }
        }

        tradeItem.TraderPrice = highestTraderPrice;

        return tradeItem.TraderPrice is not null;
    }

    private static bool IsTraderAvailable(TraderClass trader)
    {
        return trader.Info.Available && !trader.Info.Disabled && trader.Info.Unlocked;
    }

    private static bool GetTraderUserItemPrice(TraderClass trader, TradeItem tradeItem,
        out TraderClass.GStruct264? singleObjectPrice, out TraderClass.GStruct264? totalPrice)
    {
        Item singleItem = tradeItem.Item.CloneItem();
        singleItem.StackObjectsCount = 1;
        singleObjectPrice = trader.GetUserItemPrice(singleItem);

        totalPrice = trader.GetUserItemPrice(tradeItem.Item);

        return singleObjectPrice is not null;
    }

    private static double? GetCurrencyCourse(TraderClass trader, MongoID? currencyId)
    {
        if (!currencyId.HasValue)
            return null;

        double? result = null;

        switch (Plugin.Configuration?.CurrencyConversionMode?.Value ?? CurrencyConversion.Handbook)
        {
            case CurrencyConversion.Traders:
                if (currencyId.ToString() == SptConstants.CurrencyIds.Euros)
                    result = CurrencyPurchasePricesService.Instance.CurrencyPurchasePrices?.EUR;

                if (currencyId.ToString() == SptConstants.CurrencyIds.Dollars)
                    result = CurrencyPurchasePricesService.Instance.CurrencyPurchasePrices?.USD;

                break;
        }

        if (!result.HasValue)
            result = trader.GetSupplyData()?.CurrencyCourses[currencyId.Value];

        return result ?? 1;
    }

    private static bool GetFleaPrice(TradeItem tradeItem)
    {
        if (RagfairPriceTableService.Instance.Prices is not null)
        {
            if (RagfairPriceTableService.Instance.Prices!.TryGetValue(tradeItem.Item.TemplateId, out double fleaSingleObjectPrice))
            {
                double minFleaPrice = fleaSingleObjectPrice * GetPriceRangeMin();

                double? singleObjectTaxPrice = null;
                double? totalTaxPrice = null;

                if (Plugin.Configuration!.IncludeFleaTax || Plugin.Configuration!.ShowFleaTax)
                {
                    singleObjectTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, 1, minFleaPrice, false);
                    totalTaxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(tradeItem.Item, tradeItem.ItemObjectCount, minFleaPrice, false);
                }

                TradePrice tradePrice =
                    CreateTradePrice(
                        tradeItem,
                        "Flea".Localized(null),

                        minFleaPrice,
                        null,

                        null,
                        null,

                        singleObjectTaxPrice,
                        totalTaxPrice,

                        Plugin.Configuration!.IncludeFleaTax
                    );

                tradeItem.FleaPrice = tradePrice;
            }
        }

        return tradeItem.FleaPrice is not null;
    }

    private static double GetPriceRangeMin()
    {
        return RagfairPriceRangesService.Instance.Ranges?.Default?.Min ?? 0.8d;
    }

    private static bool IsFleaTaxToggleKeyPressed()
    {
        return Plugin.Configuration!.FleaTaxToggleMode.IsEnabled()
            && Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsPressed();
    }

    private static TradePrice CreateTradePrice(TradeItem tradeItem, string traderName, double singleObjectPrice, double? totalPrice,
        double? currencyCourse, MongoID? currencyId, double? singleObjectTax = null, double? totalTax = null, bool includeFleaTax = false)
    {
        TradePrice traderPrice = new
        (
            tradeItem,
            traderName,

            singleObjectPrice,
            totalPrice,

            currencyCourse,
            currencyId,

            singleObjectTax,
            totalTax,

            includeFleaTax
        );

        return traderPrice;
    }

    private static void ShowPrice(TradeItem tradeItem, TradePrice tradePriceA, TradePrice? tradePriceB, StringBuilder text)
    {
        text.Append("<br>");

        if (tradePriceB is not null)
        {
            if (tradePriceA.GetComparePriceInRouble() > tradePriceB.GetComparePriceInRouble())
                text.Append($"<color=#{GetBestTradeColor()}>{tradePriceA.TraderName}</color>: ");
            else
                text.Append($"{tradePriceA.TraderName}: ");
        }
        else
        {
            text.Append($"<color=#{GetBestTradeColor()}>{tradePriceA.TraderName}</color>: ");
        }

        if (tradeItem.ItemSlotCount > 1 || tradeItem.Item.StackObjectsCount > 1)
        {
            text.Append($"{FormatPrice(tradePriceA.GetComparePrice(), tradePriceA.CurrencySymbol)}");
            text.Append($" Total: {FormatPrice(tradePriceA.GetTotalPrice(), tradePriceA.CurrencySymbol)}");
        }
        else
        {
            text.Append($"{FormatPrice(tradePriceA.GetTotalPrice(), tradePriceA.CurrencySymbol)}");
        }

        if (Plugin.Configuration!.ShowFleaTax && tradePriceA.HasTax())
            text.Append($" Tax: {FormatPrice(tradePriceA.GetTotalTax())}");
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

    public static bool IsActive = false;
}