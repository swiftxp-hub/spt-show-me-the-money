using System.Reflection;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.ShowMeTheMoney.Loggers;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class TraderPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() => AccessTools.FirstConstructor(typeof(TraderClass), x => true);

    [PatchPostfix]
    public static void PatchPostfix(TraderClass __instance)
    {
        SimpleStaticLogger.Instance.LogDebug($"TraderPatch.PatchPostfix");
        
        __instance.UpdateSupplyData();
    }
}