using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine.EventSystems;

namespace SwiftXP.SPT.ShowMeTheMoney.Patches;

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