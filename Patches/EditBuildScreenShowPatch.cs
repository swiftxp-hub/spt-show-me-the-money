using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class EditBuildScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.FirstMethod(typeof(EditBuildScreen),
                x => x.Name == nameof(EditBuildScreen.Show));
    }

    [PatchPostfix]
    public static void PatchPostfix(InventoryScreen __instance)
    {
        Plugin.DisableTemporary = true;
    }
}