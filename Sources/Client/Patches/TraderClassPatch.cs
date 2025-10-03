using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.SPT.ShowMeTheMoney.Client.Extensions;

namespace SwiftXP.SPT.ShowMeTheMoney.Client.Patches;

public class TraderClassPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstConstructor(typeof(TraderClass), x => true);

    [PatchPostfix]
    public static void PatchPostfix(TraderClass __instance)
    {
        __instance.UpdateSupplyData();
    }
}