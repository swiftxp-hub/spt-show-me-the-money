using EFT.UI;
using SPT.Reflection.Patching;
using System.Reflection;
using HarmonyLib;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class MenuScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(MenuScreen), x => x.Name == nameof(Tooltip.Show));

    [PatchPrefix]
    public static void PatchPrefix(Tooltip __instance)
    {
        Plugin.MainMenuLoaded = true;
    }
}