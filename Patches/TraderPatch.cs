using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

public class TraderPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() => AccessTools.FirstConstructor(typeof(TraderClass), x => true);

    [PatchPostfix]
    public static void PatchPostfix(TraderClass __instance)
    {
        Plugin.SimpleSptLogger.LogDebug($"TraderPatch.PatchPostfix");
        
        __instance.UpdateSupplyData();
    }
}