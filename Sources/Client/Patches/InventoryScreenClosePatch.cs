using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class InventoryScreenClosePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(InventoryScreen), x => x.Name == nameof(InventoryScreen.Close));

    [PatchPrefix]
    public static void PatchPrefix(Tooltip __instance)
    {
        Plugin.HoveredItem = null;
        SimpleTooltipShowPatch.OnClose();
    }
}