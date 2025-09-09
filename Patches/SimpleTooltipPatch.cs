using EFT.InventoryLogic;
using EFT.UI;
using SPT.Reflection.Patching;
using SwiftXP.ShowMeTheMoney.Loggers;
using SwiftXP.ShowMeTheMoney.Models;
using System.Reflection;
using UnityEngine;
using System;
using System.Text;
using SwiftXP.ShowMeTheMoney.Sessions;
using HarmonyLib;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class SimpleTooltipPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(SimpleTooltip), x => x.Name == "Show" && x.GetParameters()[0].Name == "text");

    [PatchPrefix]
    public static void PatchPrefix(SimpleTooltip __instance, ref string text, Vector2? offset, ref float delay, float? maxWidth)
    {
        SimpleStaticLogger.Instance.LogDebug($"SimpleTooltipPatch.PatchPrefix - Item: {Plugin.HoveredItem?.TemplateId}");

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

                if (hasTraderPrice && tradeItem.TraderPrice!.PriceTotal > 0d)
                {
                    ShowPrice(tradeItem, tradeItem.TraderPrice!, tradeItem.FleaPrice, textToAddToTooltip);
                }

                if (hasFleaPrice && tradeItem.FleaPrice!.PriceTotal > 0d)
                {
                    ShowPrice(tradeItem, tradeItem.FleaPrice!, tradeItem.TraderPrice, textToAddToTooltip);
                }
            }

            text += textToAddToTooltip.ToString();
        }
        catch (Exception exception)
        {
            SimpleStaticLogger.Instance.LogError($"An unexpected error occured. Message: {exception.Message}");
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
                if (userItemPrice.HasValue)
                {
                    double itemPrice = userItemPrice.Value.Amount;

                    TradePrice traderPrice =
                        CreateTradePrice(
                            trader.LocalizedName,

                            itemPrice / tradeItem.ItemSlotCount,
                            itemPrice,
                            itemPrice * tradeItem.Item.StackObjectsCount
                        );

                    if (highestTraderPrice == null || traderPrice.PricePerSlot > highestTraderPrice.PricePerSlot)
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

    private static bool GetFleaPrice(TradeItem tradeItem)
    {
        if (RagfairPriceTable.Instance.HasPrices())
        {
            if (RagfairPriceTable.Instance.Prices!.TryGetValue(tradeItem.Item.TemplateId, out double fleaPrice))
            {
                double minFleaPrice = fleaPrice * GetPriceRangeMin();
                double? taxPrice = null;

                if (IncludeFleaTax() || ShowTax())
                {
                    Item item = new(string.Empty, tradeItem.Item.Template);

                    taxPrice = FleaTaxCalculatorAbstractClass.CalculateTaxPrice(item, 1, minFleaPrice, false);

                    if(IncludeFleaTax())
                        minFleaPrice -= taxPrice.Value;
                }

                TradePrice tradePrice =
                    CreateTradePrice(
                        "Flea".Localized(null),

                        minFleaPrice / tradeItem.ItemSlotCount,
                        minFleaPrice,
                        minFleaPrice * tradeItem.Item.StackObjectsCount,

                        taxPrice
                    );

                tradeItem.FleaPrice = tradePrice;
            }
        }

        return tradeItem.FleaPrice is not null;
    }

    private static double GetPriceRangeMin()
    {
        return RagfairPriceRanges.Instance.Ranges?.Default?.Min ?? 0.8d;
    }

    private static bool IncludeFleaTax()
    {
        return Plugin.Configuration?.IncludeFleaTax?.Value ?? true;
    }

    private static TradePrice CreateTradePrice(string traderName, double itemPricePerSlot,
        double itemPricePerObject, double itemPriceTotal, double? tax = null)
    {
        TradePrice traderPrice = new
        (
            traderName,
            itemPricePerSlot,
            itemPricePerObject,
            itemPriceTotal,
            tax
        );

        return traderPrice;
    }

    private static void ShowPrice(TradeItem tradeItem, TradePrice tradePriceA, TradePrice? tradePriceB, StringBuilder text)
    {
        text.Append("<br>");

        if (tradePriceB is not null)
        {
            if (tradePriceA.PricePerSlot > tradePriceB.PricePerSlot)
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
            text.Append($"{FormatPrice(tradePriceA.PricePerSlot)}");
            text.Append($" Total: {FormatPrice(tradePriceA.PriceTotal)}");
        }
        else
        {
            text.Append($"{FormatPrice(tradePriceA.PriceTotal)}");
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