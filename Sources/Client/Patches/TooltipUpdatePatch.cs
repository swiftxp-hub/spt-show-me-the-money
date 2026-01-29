using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;
using SwiftXP.SPT.Common.ConfigurationManager;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class TooltipUpdatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(Tooltip), x => x.Name == nameof(Tooltip.Update));

    [PatchPrefix]
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public static void PatchPrefix(Tooltip __instance)
#pragma warning restore CA1707 // Identifiers should not contain underscores

    {
        if (SimpleTooltipShowPatch.PatchIsActive
            && Plugin.Configuration!.FleaTaxToggleMode.IsEnabled()
            && (Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsDown()
                || Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsUp()
                || Plugin.Configuration!.FleaTaxToggleKey.GetValue().IsPressed()))
        {
            SimpleTooltipShowPatch.Update();
        }
    }
}