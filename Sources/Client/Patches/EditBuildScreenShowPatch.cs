using System.Reflection;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class EditBuildScreenShowPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(EditBuildScreen), x => x.Name == nameof(EditBuildScreen.Show));

    [PatchPostfix]
#pragma warning disable CA1707 // Identifiers should not contain underscores

    public static void PatchPostfix(EditBuildScreen __instance)
#pragma warning restore CA1707 // Identifiers should not contain underscores

    {
        Plugin.DisableTemporary = true;
    }
}