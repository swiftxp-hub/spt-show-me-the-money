using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.SPT.Common.Loggers;

namespace SwiftXP.ShowMeTheMoney.Patches;

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