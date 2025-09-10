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

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class SimpleTooltipPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(SimpleTooltip), x => x.Name == "Show" && x.GetParameters()[0].Name == "text");

    [PatchPrefix]
    public static void PatchPrefix(SimpleTooltip __instance, ref string text, Vector2? offset, ref float delay, float? maxWidth)
    {
        Plugin.SimpleSptLogger.LogDebug($"SimpleTooltipPatch.PatchPrefix - Item: {Plugin.HoveredItem?.Id} / {Plugin.HoveredItem?.TemplateId}");

        if (IsTemporaryDisabled() || IsInsuredByTooltip(text) || IsCheckmarkTooltip(text))
            return;

        try
        {
            StringBuilder textToAddToTooltip = new();

            if (!IsPluginEnabled())
                return;

            SetToolTipDelay(ref delay);

            Item? item = Plugin.HoveredItem;
            if (item is not null && ItemMeetsRequirements(item))
            {
                TradeItem tradeItem = CreateTradeItem(item);

                bool hasTraderPrice = false;
                bool hasFleaPrice = false;

                if (IsShowTraderPricesEnabled())
                    hasTraderPrice = GetBestTraderPrice(tradeItem);

                if (IsShowFleaPricesEnabled())
                    hasFleaPrice = GetFleaPrice(tradeItem);

                if (hasTraderPrice && tradeItem.TraderPrice!.GetTotalPriceInRouble() > 0d)
                {
                    ShowPrice(tradeItem, tradeItem.TraderPrice!, tradeItem.FleaPrice, textToAddToTooltip);
                }

                if (hasFleaPrice && tradeItem.FleaPrice!.GetTotalPriceInRouble() > 0d)
                {
                    ShowPrice(tradeItem, tradeItem.FleaPrice!, tradeItem.TraderPrice, textToAddToTooltip);
                }
            }

            text += textToAddToTooltip.ToString();
        }
        catch (Exception exception)
        {
            Plugin.SimpleSptLogger.LogError($"An unexpected error occured. Message: {exception.Message}");
        }
    }

    private static bool IsTemporaryDisabled()
    {
        return Plugin.DisableTemporary;
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

    private static bool IsPluginEnabled()
    {
        return Plugin.Configuration?.EnablePlugin?.Value ?? true;
    }

    private static void SetToolTipDelay(ref float delay)
    {
        if (Plugin.Configuration?.ToolTipDelay is not null)
            delay = (float)Plugin.Configuration.ToolTipDelay.Value;
        else
            delay = 0;
    }

    private static bool ItemMeetsRequirements(Item item)
    {
        return SptSession.Session.Profile.Examined(item)
            && (!item.IsContainer || (item.IsContainer && item.IsEmpty()))
            && !(item.Owner.OwnerType != EOwnerType.Profile && item.Owner.GetType() == typeof(TraderControllerClass));
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

    private static bool IsShowTraderPricesEnabled()
    {
        return Plugin.Configuration?.ShowTraderPrices?.Value ?? true;
    }

    private static bool IsShowFleaPricesEnabled()
    {
        return Plugin.Configuration?.ShowFleaPrices?.Value ?? true;
    }

    private static bool GetBestTraderPrice(TradeItem tradeItem)
    {
        TradePrice? highestTraderPrice = null;
        foreach (TraderClass trader in SptSession.Session.Traders)
        {
            if (IsTraderAvailable(trader))
            {
                Item item = tradeItem.Item.CloneItem();
                item.StackObjectsCount = 1;

                TraderClass.GStruct264? userItemPrice = trader.GetUserItemPrice(item);
                if (userItemPrice.HasValue && (!IsRoublesOnlyEnabled() || userItemPrice.Value.CurrencyId.ToString() == "5449016a4bdc2d6f028b456f"))
                {
                    MongoID? currencyId = userItemPrice.Value.CurrencyId;
                    double? currencyCourse = GetCurrencyCourse(trader, currencyId);
                    double itemPrice = userItemPrice.Value.Amount;

                    TradePrice traderPrice =
                        CreateTradePrice(
                            trader.LocalizedName,

                            itemPrice,
                            currencyCourse,
                            currencyId
                        );

                    Plugin.SimpleSptLogger.LogDebug($"Trader: {traderPrice.TraderName} - CurrencyId: {traderPrice.CurrencyId} - Currency: {traderPrice.CurrencySymbol} - Course: {traderPrice.CurrencyCourse} - Price: {traderPrice.Price}");

                    if (highestTraderPrice == null || traderPrice.GetSlotPriceInRouble(tradeItem.ItemSlotCount) > highestTraderPrice.GetSlotPriceInRouble(tradeItem.ItemSlotCount))
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

    private static bool IsRoublesOnlyEnabled()
    {
        return Plugin.Configuration?.RoublesOnly?.Value ?? false;
    }

    private static double? GetCurrencyCourse(TraderClass trader, MongoID? currencyId)
    {
        if (!currencyId.HasValue)
            return null;

        double? result = null;

        switch (Plugin.Configuration?.CurrencyConversionMode?.Value ?? CurrencyConversion.Handbook)
        {
            case CurrencyConversion.Traders:
                if (currencyId.ToString() == "569668774bdc2da2298b4568") // EUR
                    result = CurrencyPurchasePricesService.Instance.CurrencyPurchasePrices?.EUR;

                if (currencyId.ToString() == "5696686a4bdc2da3298b456a") // USD
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
            if (RagfairPriceTableService.Instance.Prices!.TryGetValue(tradeItem.Item.TemplateId, out double fleaPrice))
            {
                double minFleaPrice = fleaPrice * GetPriceRangeMin();
                double? taxPrice = null;

                if (IncludeFleaTax() || ShowTax())
                {
                    Item item = new(string.Empty, tradeItem.Item.Template);

                    taxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(item, 1, minFleaPrice, false);

                    if (IncludeFleaTax())
                        minFleaPrice -= taxPrice.Value;
                }

                TradePrice tradePrice =
                    CreateTradePrice(
                        "Flea".Localized(null),

                        minFleaPrice,
                        null,
                        null,

                        taxPrice
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

    private static bool IncludeFleaTax()
    {
        return Plugin.Configuration?.IncludeFleaTax?.Value ?? true;
    }

    private static TradePrice CreateTradePrice(string traderName, double price,
        double? currencyCourse, MongoID? currencyId, double? tax = null)
    {
        TradePrice traderPrice = new
        (
            traderName,
            price,
            currencyCourse,
            currencyId,
            tax
        );

        return traderPrice;
    }

    private static void ShowPrice(TradeItem tradeItem, TradePrice tradePriceA, TradePrice? tradePriceB, StringBuilder text)
    {
        text.Append("<br>");

        if (tradePriceB is not null)
        {
            if (tradePriceA.GetSlotPriceInRouble(tradeItem.ItemSlotCount) > tradePriceB.GetSlotPriceInRouble(tradeItem.ItemSlotCount))
                text.Append($"<color={Plugin.HighlightColorCode}>{tradePriceA.TraderName}</color>: ");
            else
                text.Append($"{tradePriceA.TraderName}: ");
        }
        else
        {
            text.Append($"<color={Plugin.HighlightColorCode}>{tradePriceA.TraderName}</color>: ");
        }

        if (tradeItem.ItemSlotCount > 1 || tradeItem.Item.StackObjectsCount > 1)
        {
            text.Append($"{FormatPrice(tradePriceA.GetSlotPrice(tradeItem.ItemSlotCount), tradePriceA.CurrencySymbol)}");
            text.Append($" Total: {FormatPrice(tradePriceA.Price, tradePriceA.CurrencySymbol)}");
        }
        else
        {
            text.Append($"{FormatPrice(tradePriceA.Price, tradePriceA.CurrencySymbol)}");
        }

        if (ShowTax() && tradePriceA.Tax is not null)
            text.Append($" Tax: {FormatPrice(tradePriceA.Tax.Value)}");
    }

    private static bool ShowTax()
    {
        return Plugin.Configuration?.ShowFleaTax?.Value ?? true;
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
}