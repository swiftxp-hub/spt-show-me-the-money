using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class TooltipUpdatePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(Tooltip), x => x.Name == nameof(Tooltip.Update));

    private static bool s_fleaTaxIsToggled;

    [PatchPrefix]
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public static void PatchPrefix(Tooltip __instance)
#pragma warning restore CA1707 // Identifiers should not contain underscores

    {
        if (SimpleTooltipShowPatch.PatchIsActive
            && PluginContextDataHolder.Current!.Configuration!.FleaTaxToggleMode.IsEnabled())
        {
            if (IsFleaTaxToggleKeyPressed())
            {
                if (!s_fleaTaxIsToggled)
                {
                    s_fleaTaxIsToggled = true;
                    SimpleTooltipShowPatch.Update();
                }
            }
            else
            {
                if (s_fleaTaxIsToggled)
                {
                    s_fleaTaxIsToggled = false;
                    SimpleTooltipShowPatch.Update();
                }
            }
        }
        else
        {
            s_fleaTaxIsToggled = false;
        }
    }

    private static bool IsFleaTaxToggleKeyPressed()
    {
        return PluginContextDataHolder.Current!.Configuration!.FleaTaxToggleKey.GetValue().IsDown()
            || PluginContextDataHolder.Current!.Configuration!.FleaTaxToggleKey.GetValue().IsUp()
            || PluginContextDataHolder.Current!.Configuration!.FleaTaxToggleKey.GetValue().IsPressed();
    }
}