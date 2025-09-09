using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class EditBuildScreenClosePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.FirstMethod(typeof(EditBuildScreen),
                x => x.Name == nameof(EditBuildScreen.Close));
    }

    [PatchPostfix]
    public static void PatchPostfix(EditBuildScreen __instance)
    {
        Plugin.DisableTemporary = false;
    }
}