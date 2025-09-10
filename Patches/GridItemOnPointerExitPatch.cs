using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using SwiftXP.SPT.Common.Loggers;
using System.Reflection;
using UnityEngine.EventSystems;

namespace SwiftXP.ShowMeTheMoney.Patches;

public class GridItemOnPointerExitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod() =>
        AccessTools.FirstMethod(typeof(GridItemView), x => x.Name == "OnPointerExit");
    
    [PatchPrefix]
    static void PatchPrefix(GridItemView __instance, PointerEventData eventData)
    {
        Plugin.SimpleSptLogger.LogDebug($"GridItemOnPointerExitPatch.PatchPrefix");

        Plugin.HoveredItem = null;
    }
}