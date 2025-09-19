using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class TooltipClosePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(Tooltip), x => x.Name == "Close");

    [PatchPrefix]
    public static void PatchPrefix(Tooltip __instance)
    {
        Plugin.HoveredItem = null;
        SimpleTooltipShowPatch.OnClose();
    }
}