using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;
using SwiftXP.SPT.ShowMeTheMoney.Client.Models;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class InventoryScreenClosePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(InventoryScreen), x => x.Name == nameof(InventoryScreen.Close));

    [PatchPrefix]
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public static void PatchPrefix(Tooltip __instance)
#pragma warning restore CA1707 // Identifiers should not contain underscores

    {
        PluginContextDataHolder.SetHoveredItem(null);
        SimpleTooltipShowPatch.OnClose();
    }
}