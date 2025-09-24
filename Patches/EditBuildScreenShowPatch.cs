using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class EditBuildScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(EditBuildScreen), x => x.Name == nameof(EditBuildScreen.Show));

    [PatchPostfix]
    public static void PatchPostfix(EditBuildScreen __instance)
    {
        Plugin.DisableTemporary = true;
    }
}