using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;
using SwiftXP.SPT.Common.ConfigurationManager;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class TooltipUpdatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(Tooltip), x => x.Name == nameof(Tooltip.Update));

    [PatchPrefix]
    public static void PatchPrefix(Tooltip __instance)
    {
        if (SimpleTooltipShowPatch.IsActive
            && Plugin.Configuration!.FleaTaxToggleMode.IsEnabled()
            && (Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsDown()
                || Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsUp()
                || Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsPressed()))
        {
            SimpleTooltipShowPatch.Update();
        }
    }
}