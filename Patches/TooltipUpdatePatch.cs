using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class TooltipUpdatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(Tooltip), x => x.Name == "Update");

    [PatchPrefix]
    public static void PatchPrefix(Tooltip __instance)
    {
        if (SimpleTooltipShowPatch.IsActive
            && (Plugin.Configuration?.FleaTaxToggleMode.Value ?? false)
            && (Plugin.Configuration?.FleaTaxToggleKey.Value.IsDown() ?? false))
        {
            SimpleTooltipShowPatch.EnableFleaTax();
        }

        else if (SimpleTooltipShowPatch.IsActive
            && (Plugin.Configuration?.FleaTaxToggleMode.Value ?? false)
            && (Plugin.Configuration?.FleaTaxToggleKey.Value.IsUp() ?? false))
        {
            SimpleTooltipShowPatch.DisableFleaTax();
        }
    }
}